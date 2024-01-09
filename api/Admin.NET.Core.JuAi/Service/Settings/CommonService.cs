namespace Admin.NET.Core.JuAI.Service;
[ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 200)]
public class CommonService : AppBaseService
{
    private readonly SqlSugarRepository<AppUser> _appUserRes;
    private readonly ICache _cache;

    public CommonService(SqlSugarRepository<AppUser> appUserRes, ICache cache)
    {
        _appUserRes = appUserRes;
        _cache = cache;
    }
    public async Task<string> UsersAsync([FromBody] IList<string[]> users)
    {
        foreach (var item in users)
        {
            if (_appUserRes.IsAny(m => m.Account == item[0]))
                continue;
            AppUser appUser = new AppUser();
            appUser.Account = appUser.Phone = appUser.Email = item[0];
            appUser.Password = CryptogramUtil.Encrypt(item[1]);
            appUser.CreateTime = item[2].ToDateTime();
            appUser.TokenLastUseDate = item[3].ToDateTime();
            appUser.TokenNum = item[4].ToLong();
            appUser.TokenLastUseDate = item[5].ToDateTime();
            appUser.NickName = "聚AI_" + item[0].Substring(item[0].Length - 4);
            await _appUserRes.InsertAsync(appUser);
        }
        return "同步成功";
    }
}
