

namespace Admin.NET.Core.JuAI.Service;
[ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 1)]
public class ChatgptService : AppBaseService, IScoped
{
    private readonly SqlSugarRepository<ChatModel> _chatModelRes;
    private readonly SqlSugarRepository<ChatPrompt> _chatPromptRes;

    private readonly SqlSugarRepository<ChatGPT> _chatGptRes;
    private readonly SqlSugarRepository<ChatGPTImage> _chatgptImageRes;
    private readonly ICache _cache;
    private readonly UserManager _userManager;
    public ChatgptService(SqlSugarRepository<ChatModel> chatModelRes,
        SqlSugarRepository<ChatPrompt> chatPromptRes,
        SqlSugarRepository<ChatGPT> chatgptRes,
        ICache cache,
        SqlSugarRepository<ChatGPTImage> chatgptImageRes,
        UserManager userManager)
    {
        _chatModelRes = chatModelRes;
        _chatPromptRes = chatPromptRes;
        _chatGptRes = chatgptRes;
        _cache = cache;
        _chatgptImageRes = chatgptImageRes;
        _userManager = userManager;
    }
    [HttpGet("share/{chatId}"), AllowAnonymous]
    public async Task<ChatInfoOutput> GetShareChat([FromRoute] long chatId)
    {
        ChatGPT? result = await _chatGptRes.GetByIdAsync(chatId);
        return result.Adapt<ChatInfoOutput>();
    }
    [HttpGet("next/{conversationId}/{chatId}"), AllowAnonymous]
    public async Task<object?> GetNextChat([FromRoute] long chatId, [FromRoute] long conversationId)
    {
        return await _chatGptRes.AsQueryable().Where(m => m.ConversationId == conversationId && m.Id > chatId)
                                .OrderBy(m => m.Id)
                                .Select(m => new { m.Id, m.Question }).FirstAsync();
    }

    [HttpGet("prev/{conversationId}/{chatId}"), AllowAnonymous]
    public async Task<object?> GetPrevChat([FromRoute] long chatId, [FromRoute] long conversationId)
    {
        return await _chatGptRes.AsQueryable().Where(m => m.ConversationId == conversationId && m.Id < chatId)
                                .OrderByDescending(m => m.Id)
                                .Select(m => new { m.Id, m.Question }).FirstAsync();
    }

    /// <summary>
    /// 获取广场问答记录
    /// </summary>
    /// <returns></returns>
    [HttpGet("chats"), AllowAnonymous]
    public async Task<IList<ChatInfoOutput>> GetAllChatsAsync(DateTime? lastDate)
    {
        var result = await _chatGptRes.AsQueryable().Where(m => m.CreateTime < lastDate)
                                                       .OrderByDescending(m => m.CreateTime).Take(30)
                                                       .Select(m => new ChatInfoOutput() { CreateDate = m.CreateTime! }, true).ToListAsync();
        return result;
    }

    [HttpGet("chat/prompts"), AllowAnonymous]
    public IList<ChatPrompt> GetChatGptPrompts()
    {
        return _chatPromptRes.AsQueryable().Where(m => SqlFunc.IsNullOrEmpty(m.CreateUserId)).ToList();
    }
    public async Task<ChatPrompt> GetChatGptPrompt(long promptId)
    {
        return await _chatPromptRes.GetByIdAsync(promptId);
    }

    public async Task<bool> InsertUpdateChatGpt(ChatGPT model)
    {
        if (model.Answer.IsNullOrEmpty() || model.Question.IsNullOrEmpty()) return false;
        model.IsRepeat = model.Id > 0;
        var result = await _chatGptRes.AsSugarClient().Storageable(model).ExecuteCommandAsync();
        return result > 0;
    }
    public async Task<bool> InsertUpdateChatImage(ChatGPTImage model)
    {
        var result = await _chatgptImageRes.AsSugarClient().Storageable(model).ExecuteCommandAsync();
        return result > 0;
    }
    public async Task<List<ChatMessage>> GetConversationChatGpt(ChatPromptReq chatPromptReq)
    {
        List<ChatMessage> messages = new()
        {
            ChatMessage.FromSystem("You are ChatGPT, a large language model trained by OpenAI. Follow the user\'s instructions carefully. Respond using markdown")
        };
        var maxContext = (chatPromptReq.Options.UseContext && chatPromptReq.Options.MaxContext > 0) ? chatPromptReq.Options.MaxContext.Value : 0;
        if (maxContext > 0)
        {
            var dbchats = await _chatGptRes.AsQueryable()
                                      .Where(m => m.ConversationId == chatPromptReq.Options.ConversationId && m.CreateUserId == _userManager.UserId)
                                      .OrderByDescending(x => x.Id).Take(maxContext).ToListAsync();
            dbchats.Reverse();
            foreach (var item in dbchats)
            {
                messages.Add(ChatMessage.FromUser(item.Question));
                messages.Add(ChatMessage.FromAssistant(item.Answer));
            }
        }
        messages.Add(ChatMessage.FromUser(chatPromptReq.Prompt));
        if (!chatPromptReq.Options.RolePrompt.IsNullOrWhiteSpace())
        {
            messages.Add(ChatMessage.FromUser(chatPromptReq.Options.RolePrompt));
            messages.Add(ChatMessage.FromAssistant("好的，请讲。"));
        }
        return messages;
    }

}
