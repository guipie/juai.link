namespace Admin.NET.Core.JuAI.Service;

/// <summary>
/// 用户登录参数
/// </summary>
public class AppLoginInput
{
    /// <summary>
    /// 账号
    /// </summary>
    /// <example>admin</example>
    [Required(ErrorMessage = "账号不能为空"), MinLength(2, ErrorMessage = "账号不能少于2个字符")]
    public string Account { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    /// <example>123456</example>
    public string? Password { get; set; }


    /// <summary>
    /// 验证码
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 邀请码
    /// </summary>
    public string? U { get; set; }
}