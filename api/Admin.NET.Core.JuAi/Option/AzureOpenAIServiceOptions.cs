using Microsoft.Extensions.Configuration;

namespace Admin.NET.Core.JuAI;
public sealed class AzureOpenAIServiceOptions : IConfigurableOptions
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
    public string DeploymentId { get; set; } = string.Empty;
    public string ResourceName { get; set; } = string.Empty;
    public string ProviderType { get; set; } = string.Empty;

    public string? ApiVersion { get; set; }

    //public void PostConfigure(ChatGPTOptions options, IConfiguration configuration)
    //{
    //    options.ApiKey = ApiKey;
    //    options.BaseDomain = BaseDomain;
    //    options.Organization = Organization;
    //}
}
