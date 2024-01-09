using Microsoft.Extensions.Configuration;

namespace Admin.NET.Core.JuAI;
public sealed class MidjourneyOptions : IConfigurableOptions
{

    [Required(ErrorMessage = "Midjourney接口地址不能为空")]
    /// <summary>
    /// 代理地址
    /// </summary>
    public string ApiDomain { get; set; } = string.Empty;
    public string NotifyHookUrl { get; set; } = string.Empty;


    //public void PostConfigure(MidjourneyOptions options, IConfiguration configuration)
    //{
    //    options.ApiDomain = ApiDomain;
    //    options.NotifyHookUrl = NotifyHookUrl;
    //}
}
