
namespace Admin.NET.Core.JuAI.Service;
public class ImagineTaskReq
{
    /// <summary>
    /// 垫图base64
    /// </summary>
    public string base64 { get; set; } = string.Empty;
    /// <summary>
    /// 回调地址, 为空时使用全局notifyHook
    /// </summary>
    public string notifyHook { get; set; } = string.Empty;
    /// <summary>
    /// 提示词,示例值(Cat)
    /// </summary>
    public string prompt { get; set; } = string.Empty;
    public string promptEn { get; set; } = string.Empty;

    /// <summary>
    /// 自定义参数
    /// </summary>
    public string state { get; set; } = string.Empty;
}
