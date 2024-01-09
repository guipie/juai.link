namespace Admin.NET.Core.JuAI.Service;

/// <summary>
/// 用户
/// </summary>
public class UserInfoOutput
{
    public UserInfoOutput() { }
    public UserInfoOutput(long id, string nick, string? avatar)
    {
        NickName = nick;
        Id = id;
        Avatar = avatar;
    }
    public string NickName { get; set; } = string.Empty;

    public string? Avatar { get; set; }

    public long Id { get; set; }
}