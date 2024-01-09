namespace Admin.NET.Core.JuAI.Service;

/// <summary>
/// 用户登录结果
/// </summary>
public class AppLoginOutput : AppUserInfo
{
    public long UserId { get; set; }
    /// <summary>
    /// 令牌Token
    /// </summary> 
    public string RefreshToken { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;

}

public class AppUserInfo
{

    public string Account { get; set; } = string.Empty;
    public string NickName { get; set; } = string.Empty;

    public string? Avatar { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public int Sex { get; set; }
    public long TokenNum { get; set; } = 0;
    public PayTypeEnum PayType { get; set; }
}