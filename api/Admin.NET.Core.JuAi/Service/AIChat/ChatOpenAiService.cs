using Admin.NET.Core.JuAI.Service.AI;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.Tokenizer.GPT3;
using Yitter.IdGenerator;

namespace Admin.NET.Core.JuAI;

/// <summary>
/// OpenAi相关接口
/// </summary>
[ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 1)]
public class ChatOpenAiService : AppBaseService, IAiChatService
{

    private readonly SqlSugarRepository<ChatModel> _modelRep;
    private readonly ChatgptService _chatgptService;
    private readonly IOpenAIService _openAIService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserTokenService _userTokenService;
    public ChatOpenAiService(SqlSugarRepository<ChatModel> modelRep, ChatgptService chatgptService, IOpenAIService openAIService, IHttpContextAccessor httpContextAccessor, UserTokenService userTokenService)
    {
        _modelRep = modelRep;
        _chatgptService = chatgptService;
        _openAIService = openAIService;
        _httpContextAccessor = httpContextAccessor;
        _userTokenService = userTokenService;
    }

    [TypeFilter(typeof(TokenUsageFilter), Arguments = new object[] { TokenUsageType.Chat })]
    [NonUnify]
    public async Task ChatAsync([FromBody] ChatPromptReq openAiPrompts)
    {
        var firstChunk = true;
        openAiPrompts.Options.ConversationId = openAiPrompts.Options.ConversationId;
        ChatGPT chatGPT = new()
        {
            Model = openAiPrompts.Model,
            Question = openAiPrompts.Prompt,
            Id = openAiPrompts.Options.ChatDbId ?? YitIdHelper.NextId(),
            ConversationId = (long)openAiPrompts.Options.ConversationId
        };
        List<ChatMessage> messages = await _chatgptService.GetConversationChatGpt(openAiPrompts);
        if (!openAiPrompts.Options.RolePrompt.IsNullOrEmpty())
            messages.Insert(0, new ChatMessage(StaticValues.ChatMessageRoles.Assistant, openAiPrompts.Options.RolePrompt));

        var completionResult = _openAIService.ChatCompletion.CreateCompletionAsStream(new ChatCompletionCreateRequest()
        {
            TopP = openAiPrompts.Top_p,
            Temperature = openAiPrompts.Temperature,
            Messages = messages,
            Model = openAiPrompts.Model,
        }, null);
        chatGPT.ReqNum = messages.Select(m => TokenizerGpt3.TokenCount(m.Content)).Sum();
        // 设置响应头，指定 SSE 的内容类型
        //_httpContextAccessor.HttpContext!.Response.Headers.Add("Cache-Control", "no-cache");
        //_httpContextAccessor.HttpContext!.Response.Headers.Add("Content-Type", "text/event-stream");
        try
        {
            await foreach (var completion in completionResult)
            {
                Console.WriteLine("-----------------开始--------------------");
                Console.WriteLine("返回结果-completion:" + completion.ToJson());
                if (completion.Successful)
                {
                    var choice = completion.Choices.FirstOrDefault();
                    bool isStop = choice?.FinishReason == "stop";
                    chatGPT.Answer += choice?.Message.Content ?? "";
                    if (chatGPT.Answer.IsNullOrWhiteSpace()) continue;
                    chatGPT.ResNum = TokenizerGpt3.TokenCount(chatGPT.Answer);
                    var chatRes = new ChatRes(chatdbId: chatGPT.Id, chatGPT.Answer, StaticValues.ChatMessageRoles.Assistant, chatGPT.ConversationId);
                    if (isStop)
                    {
                        Console.WriteLine($"-----------------请求token数：{chatGPT.ReqNum},返回token数：{chatGPT.ResNum}--------------------");
                        return;
                    }
                    await _httpContextAccessor.HttpContext!.Response.WriteAsync((firstChunk ? "" : "\n") + chatRes.ToJson());
                    firstChunk = false;
                    await _httpContextAccessor.HttpContext!.Response.Body.FlushAsync();

                }
                else
                {
                    //throw Oops.Bah($"出错了({completion.Error?.Message}),请重试（联系vx:15100305）"); 
                    await _httpContextAccessor.HttpContext!.Response.WriteAsync((firstChunk ? "" : "\n") + new ChatRes(chatGPT.Id, $"出错了({completion.Error?.Message}),请重试（联系vx:15100305）", StaticValues.ChatMessageRoles.Assistant, chatGPT.ConversationId).ToJson());
                    firstChunk = false;
                    await _httpContextAccessor.HttpContext!.Response.Body.FlushAsync();
                }
                Console.WriteLine("-----------------结束--------------------");
            }
        }
        catch (Exception ex)
        {
            Log.CreateLogger<DrawingOpenAIService>().LogError("openAI聊天出错,错误信息:" + ex.Message, openAiPrompts.ToJson(), ex.StackTrace);
            await _httpContextAccessor.HttpContext!.Response.WriteAsync((firstChunk ? "" : "\n") + new ChatRes(chatGPT.Id, $"出错了({ex.Message}),请重试（联系vx:15100305）", StaticValues.ChatMessageRoles.Assistant, chatGPT.ConversationId).ToJson());
            firstChunk = false;
            await _httpContextAccessor.HttpContext!.Response.Body.FlushAsync();
        }
        finally
        {
            await _chatgptService.InsertUpdateChatGpt(chatGPT);
            await _userTokenService.SetMinusUserToken(chatGPT.ReqNum + chatGPT.ResNum);
            await _httpContextAccessor.HttpContext!.Response.Body.FlushAsync();
            await _httpContextAccessor.HttpContext!.Response.CompleteAsync().ConfigureAwait(false);
        }
    }
}
