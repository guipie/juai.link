using Admin.NET.Core.JuAI.Service.AI;
using Furion.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OpenAI.Interfaces;
using OpenAI.ObjectModels.ResponseModels.ImageResponseModel;

namespace Admin.NET.Core.JuAI.Service;
[ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 2)]
public class DrawingOpenAIService(UserTokenService userTokenService,
    ChatgptService chatgptService, IOpenAIService openAIService,
    JuFileService juFileService, IHttpContextAccessor httpContextAccessor, GptsService gptsService
        ) : AppBaseService, IAiDrawingService
{
    public UserTokenService _userTokenService = userTokenService;
    private readonly ChatgptService _chatgptService = chatgptService;
    private readonly IOpenAIService _openAIService = openAIService;
    private readonly JuFileService _juFileService = juFileService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly GptsService _gptsService = gptsService;

    [TypeFilter(typeof(TokenUsageFilter), Arguments = new object[] { TokenUsageType.ChatImage })]
    public async Task DrawingAsync([FromBody] DrawingReq drawingReq)
    {
        bool firstChunk = true;
        try
        {
            // 设置响应头，指定 SSE 的内容类型
            _httpContextAccessor.HttpContext!.Response.Headers.Append("Cache-Control", "no-cache");
            _httpContextAccessor.HttpContext!.Response.Headers.Append("Content-Type", "text/event-stream");

            drawingReq.User = "Designer";
            drawingReq.Model = drawingReq.Model.IsNullOrEmpty() ? "dall-e-3" : drawingReq.Model;
            string promptEn = await _gptsService.TranslationPrompt(drawingReq.Prompt, drawingReq.IsOpt);
            if (drawingReq.Tags?.Length > 0) { promptEn += ";" + drawingReq.Tags.Join(","); } //累加风格
            string promptCopy = drawingReq.Prompt;
            drawingReq.Prompt = promptEn;
            Task<ImageCreateResponse> res = _openAIService.Image.CreateImage(drawingReq.Adapt<ImageCreateRequest>());
            var now = DateTime.Now;
            while (!res.IsCompleted)
            {
                var currentTime = DateTime.Now;
                await Console.Out.WriteLineAsync("开始绘图" + (currentTime - now).Seconds);
                await Task.Delay(1000); // 模拟延时 
                                        // 推送时间信息给客户端
                await _httpContextAccessor.HttpContext!.Response.WriteAsync((firstChunk ? "" : "\n") + new { useSeconds = (currentTime - now).Seconds }.ToJson());
                await _httpContextAccessor.HttpContext!.Response.Body.FlushAsync();
                firstChunk = false;
            }
            var useTokens = 20000;
            var sourceUrl = res.Result.Results.Select(m => m.Url).FirstOrDefault();
            var chatImageAddModel = new ChatGPTImage
            {
                Id = drawingReq.Id ?? Yitter.IdGenerator.YitIdHelper.NextId(),
                Prompt = promptCopy,
                PromptEn = promptEn,
                SourceUrl = sourceUrl,
                Num = useTokens,
                Model = drawingReq.Model,
                DrawingType = DrawingImageType.text_image,
                RequestParameters = JObject.FromObject(drawingReq),
                ResponseParameters = JObject.FromObject(res.Result)
            };
            if (res.Result.Successful && !sourceUrl.IsNullOrWhiteSpace())
            {
                chatImageAddModel.SourceUrl = sourceUrl;
                chatImageAddModel.IsSuccess = true;
            }
            else
                throw new Exception("OpenAI服务器出现异常.");
            await _chatgptService.InsertUpdateChatImage(chatImageAddModel);
            await _juFileService.AddFileToDb(chatImageAddModel.Id, [sourceUrl], DrawingImageType.text_image.ToString(), FileRelationEnum.ChatgptImage);
            await _userTokenService.SetMinusUserToken(useTokens);
            await _httpContextAccessor.HttpContext!.Response.WriteAsync("\n" + new { url = chatImageAddModel.SourceUrl }.ToJson());
            await _httpContextAccessor.HttpContext!.Response.Body.FlushAsync();
            await _httpContextAccessor.HttpContext!.Response.CompleteAsync();
        }
        catch (Exception error)
        {
            Log.CreateLogger<DrawingOpenAIService>().LogError("chatgpt绘画出错,错误信息:" + error.Message, drawingReq.ToJson(), error.StackTrace);
            await _httpContextAccessor.HttpContext!.Response.WriteAsync((firstChunk ? "" : "\n") + new { err = ApplicationAppConst.ErrorrMsg }.ToJson());
            await _httpContextAccessor.HttpContext!.Response.Body.FlushAsync();
            await _httpContextAccessor.HttpContext!.Response.CompleteAsync();
        }
    }

}
