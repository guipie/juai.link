namespace Admin.NET.Core.JuAI.Service;

/// <summary>
/// 支付前请求
/// </summary>
public class UserPayInfoInput
{
    /// <summary>
    /// 账号
    /// </summary>
    /// <example>admin</example>
    [Required(ErrorMessage = "账号不能为空"), MinLength(2, ErrorMessage = "账号不能少于2个字符")]
    public string Account { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public long Num { get; set; }

    public double RMB { get; set; }

    public string OrderNo { get; set; } = string.Empty;

}