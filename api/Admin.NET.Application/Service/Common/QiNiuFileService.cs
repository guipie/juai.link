

using Microsoft.Extensions.Options;
using OnceMi.AspNetCore.OSS;

namespace Admin.NET.Application.JuAI;
/// <summary>
/// 七牛文件服务扩展
/// </summary>
[ApiDescriptionSettings(ApplicationAppConst.GroupName, Order = 800)]
public class QiNiuFileService : IDynamicApiController, ITransient
{
    private readonly OSSProviderOptions _OSSProviderOptions;
    private readonly IOSSService _OSSService;
    public QiNiuFileService(SqlSugarRepository<MidjourneyNotify> midRep, IOptions<OSSProviderOptions> oSSProviderOptions, IOSSServiceFactory ossServiceFactory)
    {
        _OSSProviderOptions = oSSProviderOptions.Value;
        if (_OSSProviderOptions.IsEnable)
            _OSSService = ossServiceFactory.Create(Enum.GetName(_OSSProviderOptions.Provider));
    }

}
