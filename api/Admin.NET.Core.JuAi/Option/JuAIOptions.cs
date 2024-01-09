using Microsoft.Extensions.Configuration;

namespace Admin.NET.Core.JuAI;
public sealed class ChatGPTOptions : IConfigurableOptions
{

    [Required(ErrorMessage = "key不能为空")]
    /// <summary>
    /// 访问KEY
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    [Required(ErrorMessage = "代理地址不能为空")]
    /// <summary>
    /// 代理地址
    /// </summary>
    public string BaseDomain { get; set; } = string.Empty;

    public string? Organization { get; set; }

}

public sealed class XunfeiOptions : IConfigurableOptions
{

    [Required(ErrorMessage = "AppId不能为空")]
    public string AppId { get; set; } = string.Empty;


    [Required(ErrorMessage = "ApiKey不能为空")]
    public string ApiKey { get; set; } = string.Empty;

    [Required(ErrorMessage = "ApiSecret不能为空")]
    public string ApiSecret { get; set; } = string.Empty;

}
public sealed class BaiduOptions : IConfigurableOptions
{

    [Required(ErrorMessage = "ApiKey不能为空")]
    public string ApiKey { get; set; } = string.Empty;

    [Required(ErrorMessage = "ApiSecret不能为空")]
    public string ApiSecret { get; set; } = string.Empty;

}

public sealed class AlibabaOptions : IConfigurableOptions
{

    [Required(ErrorMessage = "DashscopeKey不能为空")]
    public string DashscopeKey { get; set; } = string.Empty;

}

public sealed class TencentOptions : IConfigurableOptions
{

    [Required(ErrorMessage = "SecretId不能为空")]
    public string SecretId { get; set; } = string.Empty;

    [Required(ErrorMessage = "SecretKey不能为空")]
    public string SecretKey { get; set; } = string.Empty;

}

public sealed class LeptonaiOptions : IConfigurableOptions
{

    [Required(ErrorMessage = "Servers不能为空")]
    public string[] Servers { get; set; }


    [Required(ErrorMessage = "Keys不能为空")]
    public string[] Keys { get; set; }

}
