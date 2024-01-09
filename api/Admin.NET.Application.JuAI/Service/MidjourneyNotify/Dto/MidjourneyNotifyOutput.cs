namespace Admin.NET.Application.JuAI;

/// <summary>
/// Mid绘画输出参数
/// </summary>
public class MidjourneyNotifyOutput
{
    /// <summary>
    /// Id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public string Action { get; set; }

    /// <summary>
    /// 中文
    /// </summary>
    public string Prompt { get; set; }

    /// <summary>
    /// 英文
    /// </summary>
    public string? PromptEn { get; set; }

    /// <summary>
    /// description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// state
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// 提交时间
    /// </summary>
    public long? SubmitTime { get; set; }

    /// <summary>
    /// start_time
    /// </summary>
    public long? StartTime { get; set; }

    /// <summary>
    /// finish_time
    /// </summary>
    public long? FinishTime { get; set; }

    /// <summary>
    /// image_url
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// progress
    /// </summary>
    public string? Progress { get; set; }

    /// <summary>
    /// fail_reason
    /// </summary>
    public string? FailReason { get; set; }

    /// <summary>
    /// properties
    /// </summary>
    public string? Properties { get; set; }

    /// <summary>
    /// 云地址
    /// </summary>
    public string? OssUrl { get; set; }
    public ContentStatusEnum ContentStatus { get; set; }
    public SysFile OssUrlAttachment { get; set; }

}


