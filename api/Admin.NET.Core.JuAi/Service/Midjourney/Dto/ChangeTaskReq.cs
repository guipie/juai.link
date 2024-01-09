
namespace Admin.NET.Core.JuAI.Service;
public class ChangeTaskReq
{
    /// <summary>
    /// UPSCALE(放大); VARIATION(变换); REROLL(重新生成),可用值:UPSCALE,VARIATION,REROLL,示例值(UPSCALE)
    /// </summary>
    public string action { get; set; } = string.Empty;
    /// <summary>
    /// 回调地址, 为空时使用全局notifyHook
    /// </summary>
    public string notifyHook { get; set; } = string.Empty;
    /// <summary>
    /// 	序号(1~4), action为UPSCALE,VARIATION时必传,示例值(1)
    /// </summary>
    public int index { get; set; }

    public string taskId { get; set; } = string.Empty;

    /// <summary>
    /// 自定义参数
    /// </summary>
    public string state { get; set; } = string.Empty;
}
