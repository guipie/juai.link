
namespace Admin.NET.Core.JuAI.Service;
[ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 100)]
public class UpgradeService : AppBaseService
{
    private readonly SqlSugarRepository<AppUpgrade> _appUpgradeRes;
    public UpgradeService(SqlSugarRepository<AppUpgrade> appUpgradeRes)
    {
        _appUpgradeRes = appUpgradeRes;
    }
    [AllowAnonymous]
    public AppUpgrade GetNewst()
    {
        return _appUpgradeRes.AsQueryable().OrderByDescending(m => m.Id).First();
    }
}
