
using System.Text.RegularExpressions;

namespace Admin.NET.Core.JuAI;

/// <summary>
/// 用户表
/// </summary>
[SugarTable("AppUser", "用户表"), IncreTable]
[Tenant("app")]
public class AppUser : EntityTenant
{
    public AppUser() { }
    public AppUser(string account, string? pwd)
    {
        Account = account.StrRandom();
        if (Regex.IsMatch(account, @"^1[3456789]\d{9}$"))
            Phone = account;
        else
            Email = account;
        Password = pwd ?? account[^6..];
        TenantId = long.Parse(SqlSugarConst.MainConfigId);
        NickName = string.Concat("聚AI_", account.AsSpan(account.Length - 4));
    }
    /// <summary>
    /// 账号
    /// </summary>
    [SugarColumn(ColumnDescription = "账号", Length = 32)]
    [Required, MaxLength(32)]
    public string Account { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [SugarColumn(ColumnDescription = "密码", Length = 2000)]
    [Required, MaxLength(2000)]
    [System.Text.Json.Serialization.JsonIgnore]
    [JsonIgnore]
    public string Password { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    [SugarColumn(ColumnDescription = "昵称", Length = 32)]
    [Required, MaxLength(32)]
    public string NickName { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    [SugarColumn(ColumnDescription = "头像", Length = 512)]
    [MaxLength(512)]
    public string? Avatar { get; set; }

    /// <summary>
    /// 性别-男_1、女_2
    /// </summary>
    [SugarColumn(ColumnDescription = "性别")]
    public int Sex { get; set; } = 0;

    /// <summary>
    /// 年龄
    /// </summary>
    [SugarColumn(ColumnDescription = "年龄")]
    public int Age { get; set; }

    /// <summary>
    /// 出生日期
    /// </summary>
    [SugarColumn(ColumnDescription = "出生日期")]
    public DateTime? Birthday { get; set; }


    /// <summary>
    /// 手机号码
    /// </summary>
    [SugarColumn(ColumnDescription = "手机号码", Length = 64)]
    [Required, MaxLength(64)]
    public string Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(ColumnDescription = "邮箱", Length = 64)]
    [MaxLength(64)]
    public string? Email { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public StatusEnum Status { get; set; } = StatusEnum.Enable;


    [SugarColumn(ColumnDescription = "token数量")]
    public long TokenNum { get; set; } = 0;


    [SugarColumn(ColumnDescription = "token最后使用时间")]
    public DateTime? TokenLastUseDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 256)]
    [MaxLength(256)]
    public string? Remark { get; set; }

    /// <summary>
    /// 账号类型
    /// </summary>
    [SugarColumn(ColumnDescription = "账号类型")]
    public AccountTypeEnum AccountType { get; set; } = AccountTypeEnum.NormalUser;


    /// <summary>
    /// 最新登录Ip
    /// </summary>
    [SugarColumn(ColumnDescription = "最新登录Ip", Length = 256)]
    [MaxLength(256)]
    public string? LastLoginIp { get; set; }

    /// <summary>
    /// 最新登录地点
    /// </summary>
    [SugarColumn(ColumnDescription = "最新登录地点", Length = 128)]
    [MaxLength(128)]
    public string? LastLoginAddress { get; set; }

    /// <summary>
    /// 最新登录时间
    /// </summary>
    [SugarColumn(ColumnDescription = "最新登录时间")]
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// 最新登录设备
    /// </summary>
    [SugarColumn(ColumnDescription = "最新登录设备", Length = 128)]
    [MaxLength(128)]
    public string? LastLoginDevice { get; set; }

    [SugarColumn(ColumnDescription = "邀请码", Length = 6)]
    public string? Vcode { get; set; }


    [SugarColumn(ColumnDescription = "付费类型")]
    public PayTypeEnum PayType { get; set; } = PayTypeEnum.Baipiao;


    [SugarColumn(ColumnDescription = "付费时间")]
    public DateTime? PayDateTime { get; set; }
}