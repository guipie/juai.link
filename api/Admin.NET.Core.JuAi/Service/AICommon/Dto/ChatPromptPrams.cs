namespace Admin.NET.Core.JuAI.Service;
public class ChatGptReqOptions
{

    public bool UseContext { get; set; } = true;
    [Required(ErrorMessage = "未找到会话ID，刷新重试")]
    public long ConversationId { get; set; }
    public long? ChatDbId { get; set; }
    public string? RolePrompt { get; set; }
    public int? MaxContext { get; set; }
}
public class ChatPromptReq
{

    public string Prompt { get; set; } = string.Empty;

    public ChatGptReqOptions Options { get; set; } = new ChatGptReqOptions();
    public float Temperature { get; set; }
    public float Top_p { get; set; }
    public string Model { get; set; } = string.Empty;
}
public class ChaRoomReq : ChatPromptReq
{
    public string Url { get; set; }
}

/// <summary>
/// 使用小写，不知道为何 fromform接收参数时是区分大小写的。
/// </summary>
public class PromptReponse
{
    public long? id { get; set; }
    public string type { get; set; } = "system";

    //[Required(ErrorMessage = "名称必填")]
    public string title { get; set; }

    //[Required(ErrorMessage = "提示词必填"), MaxLength(4000, ErrorMessage = "专栏简介不能大于4000个字符")]
    public string prompt { get; set; }

    public int maxContext { get; set; }

    public string promptExtend { get; set; } = string.Empty;
    public string avatar { get; set; } = string.Empty;
    public IFormFile? file { get; set; }
    public string initMessage { get; set; } = string.Empty;

    public string? tags { get; set; }

    public string model { get; set; } = string.Empty;
    public string vendor { get; set; } = string.Empty;
}