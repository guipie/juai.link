using Admin.NET.Core.JuAI.Service.AI;
using Furion.Logging;
using Furion.RemoteRequest.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using OpenAI.Interfaces;
using OpenAI.ObjectModels.ResponseModels.ImageResponseModel;

namespace Admin.NET.Core.JuAI.Service;
[ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 2)]
public class DrawingYiShuZiService(UserTokenService userTokenService,
    ChatgptService chatgptService, IOpenAIService openAIService,
    SysFileService fileService, IHttpContextAccessor httpContextAccessor,
    GptsService gptsService, IOptions<LeptonaiOptions> options
        ) : AppBaseService
{
    public UserTokenService _userTokenService = userTokenService;
    private readonly ChatgptService _chatgptService = chatgptService;
    private readonly IOpenAIService _openAIService = openAIService;
    private readonly SysFileService _fileService = fileService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly GptsService _gptsService = gptsService;
    private readonly LeptonaiOptions _options = options.Value;

    [TypeFilter(typeof(TokenUsageFilter), Arguments = new object[] { TokenUsageType.ChatImage })]
    public async Task DrawingAsync([FromBody] DrawingYiShuZiReq drawingReq)
    {
        bool firstChunk = true;
        try
        {
            // 设置响应头，指定 SSE 的内容类型
            _httpContextAccessor.HttpContext!.Response.Headers.Append("Cache-Control", "no-cache");
            _httpContextAccessor.HttpContext!.Response.Headers.Append("Content-Type", "text/event-stream");
            string promptEn = await _gptsService.TranslationPrompt(drawingReq.Prompt, drawingReq.IsOpt);
            string promptCopy = drawingReq.Prompt;
            drawingReq.Prompt = promptEn;
            if (drawingReq.Tags?.Length > 0) { promptEn += ";" + drawingReq.Tags.Join(","); } //累加风格
            if (!drawingReq.Size.IsNullOrWhiteSpace() && drawingReq.Size != "1024x1024")
            {
                drawingReq.Prompt += "; the generated size is" + drawingReq.Size;
            }
            var res = _options.Servers.FirstOrDefault().SetHeaders(headers: new Dictionary<string, string>() { ["Authorization"] = "Bearer " + _options.Keys.FirstOrDefault() })
                                .SetBody(drawingReq.ToJson()).PostAsStringAsync();
            var now = DateTime.Now;
            while (!res.IsCompleted)
            {
                int seconds = (DateTime.Now - now).Seconds;
                await Console.Out.WriteLineAsync("开始绘图" + seconds);
                await Task.Delay(1000); // 模拟延时 
                                        // 推送时间信息给客户端
                await _httpContextAccessor.HttpContext!.Response.WriteAsync((firstChunk ? "" : "\n") + new { useSeconds = seconds }.ToJson());
                await _httpContextAccessor.HttpContext!.Response.Body.FlushAsync();
                firstChunk = false;
                if (seconds > 60 * 10) return;
            }
            var useTokens = 20000;
            var sourceUrl = string.Empty;
            try
            {
                sourceUrl = JSON.Deserialize<string[]>(res.Result).FirstOrDefault();
                drawingReq.ControlImage = null; //base64 太大暂不存。
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + res.Result);
            }
            var chatImageAddModel = new ChatGPTImage
            {
                Id = Yitter.IdGenerator.YitIdHelper.NextId(),
                Prompt = promptCopy,
                PromptEn = promptEn,
                SourceUrl = sourceUrl,
                Num = useTokens,
                Model = drawingReq.Model,
                DrawingType = drawingReq.DrawingType,
                RequestParameters = JObject.FromObject(drawingReq),
                ResponseParameters = JObject.FromObject(new { result = res.Result })
            };
            if (!sourceUrl.IsNullOrWhiteSpace())
            {
                chatImageAddModel.SourceUrl = sourceUrl;
                chatImageAddModel.IsSuccess = true;
            }
            var file = await _fileService.UploadFileFromBase64(new UploadFileFromBase64Input() { ContentType = "image/png", FileDataBase64 = sourceUrl, Path = drawingReq.DrawingType.ToString() });
            chatImageAddModel.OssUrl = file.Url;
            await _chatgptService.InsertUpdateChatImage(chatImageAddModel);
            await _userTokenService.SetMinusUserToken(useTokens);
            await _httpContextAccessor.HttpContext!.Response.WriteAsync("\n" + new { url = file.Url }.ToJson());
            await _httpContextAccessor.HttpContext!.Response.Body.FlushAsync();
            await _httpContextAccessor.HttpContext!.Response.CompleteAsync();
        }
        catch (Exception error)
        {
            Log.CreateLogger<DrawingOpenAIService>().LogError("艺术字绘画出错,错误信息:" + error.Message, drawingReq.ToJson(), error.StackTrace);
            await _httpContextAccessor.HttpContext!.Response.WriteAsync((firstChunk ? "" : "\n") + new { err = ApplicationAppConst.ErrorrMsg }.ToJson());
            await _httpContextAccessor.HttpContext!.Response.Body.FlushAsync();
            await _httpContextAccessor.HttpContext!.Response.CompleteAsync();
        }
    }

}
