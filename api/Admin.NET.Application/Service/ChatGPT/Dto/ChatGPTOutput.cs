namespace Admin.NET.Application.JuAI;

/// <summary>
/// AI会话输出参数
/// </summary>
public class ChatGPTOutput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 问题
    /// </summary>
    public string Question { get; set; }

    /// <summary>
    /// 答案
    /// </summary>
    public string Answer { get; set; }

    /// <summary>
    /// 问(token数)
    /// </summary>
    public int ReqNum { get; set; }

    /// <summary>
    /// 答(token数)
    /// </summary>
    public int ResNum { get; set; }

    /// <summary>
    /// 模型
    /// </summary>
    public string Model { get; set; }

    /// <summary>
    /// 模型
    /// </summary>
    public int ChatModelModeltype { get; set; }

    /// <summary>
    /// 会话ID
    /// </summary>
    public long ConversationId { get; set; }

    /// <summary>
    /// IsRepeat
    /// </summary>
    public bool Isrepeat { get; set; }
    public ContentStatusEnum Status { get; set; }


}


