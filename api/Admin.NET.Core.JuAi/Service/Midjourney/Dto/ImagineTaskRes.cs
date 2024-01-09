
namespace Admin.NET.Core.JuAI.Service;
public class ImagineTaskRes
{
    /// <summary>
    /// 状态码: 1(提交成功), 21(已存在), 22(排队中), other(错误)
    /// </summary>
    public string code { get; set; } = string.Empty;
    /// <summary>
    /// 描述	
    /// </summary>
    public string description { get; set; } = string.Empty;
    /// <summary>
    /// 扩展字段	
    /// </summary>
    public object? properties { get; set; }
    public string promptEn { get; set; } = string.Empty;
    public string prompt { get; set; } = string.Empty;

    /// <summary>
    /// 任务ID	
    /// </summary>
    public string result { get; set; } = string.Empty;
}
