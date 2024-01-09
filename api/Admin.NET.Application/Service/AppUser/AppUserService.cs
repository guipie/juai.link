

namespace Admin.NET.Application.JuAI;
/// <summary>
/// 聚AI用户服务
/// </summary>
[ApiDescriptionSettings(ApplicationAppConst.GroupName, Order = 100)]
public class AppUserService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<AppUser> _rep;
    private readonly SqlSugarRepository<TokenPayRecord> _repPayed;
    public AppUserService(SqlSugarRepository<AppUser> rep, SqlSugarRepository<TokenPayRecord> repPayed)
    {
        _rep = rep;
        _repPayed = repPayed;
    }

    /// <summary>
    /// 分页查询聚AI用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Page")]
    public async Task<SqlSugarPagedList<AppUserOutput>> Page(AppUserInput input)
    {
        var query = _rep.AsQueryable()
                    .WhereIF(!string.IsNullOrWhiteSpace(input.Account), u => u.Account.Contains(input.Account.Trim()))
                    .WhereIF(!string.IsNullOrWhiteSpace(input.NickName), u => u.NickName.Contains(input.NickName.Trim()))
                    .WhereIF(!string.IsNullOrWhiteSpace(input.Phone), u => u.Phone.Contains(input.Phone.Trim()))
                    .WhereIF(input.TokenNum > 0, u => u.TokenNum >= input.TokenNum)
                    .Select(u => new AppUserOutput { NickName = u.NickName }, true);
        //.Mapper(c => c.AvatarAttachment, c => c.Avatar)

        query = query.OrderBuilder(input);
        return await query.ToPagedListAsync(input.Page, input.PageSize);
    }
    [HttpPost]
    [ApiDescriptionSettings(Name = "Payed/Page")]
    public async Task<SqlSugarPagedList<AppUserPayedOutput>> PayedPage(AppUserInput input)
    {
        var query = _repPayed.AsQueryable().Where(m => m.PayedNum > 0).LeftJoin<AppUser>((pay, user) => pay.CreateUserId == user.Id)
                    .WhereIF(!string.IsNullOrWhiteSpace(input.Account), (pay, user) => user.Account.Contains(input.Account.Trim()))
                    .WhereIF(!string.IsNullOrWhiteSpace(input.NickName), (pay, user) => user.NickName.Contains(input.NickName.Trim()))
                    .WhereIF(!string.IsNullOrWhiteSpace(input.Phone), (pay, user) => user.Phone.Contains(input.Phone.Trim()))
                    .GroupBy((pay, user) => pay.CreateUserId)
                    .Select((pay, user) => new AppUserPayedOutput
                    {
                        NickName = user.NickName,
                        Account = user.Account,
                        Id = user.Id,
                        PayedCount = SqlFunc.AggregateCount(pay.Id),
                        PayedNum = SqlFunc.AggregateSumNoNull(pay.PayedNum ?? 0),
                        PayedMoney = SqlFunc.AggregateSumNoNull(pay.PayedRmb ?? 0)
                    });
        //.Mapper(c => c.AvatarAttachment, c => c.Avatar)

        query = query.OrderBuilder(input);
        return await query.ToPagedListAsync(input.Page, input.PageSize);
    }
    /// <summary>
    /// 增加聚AI用户
    /// </summary>
    /// <param name="resetUser"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "pwd/reset")]
    public async Task<int> ResetPwd(AppUserResetPwdInput resetUser)
    {
        var user = await _rep.GetFirstAsync(u => u.Account == resetUser.Account);
        user.Password = CryptogramUtil.Encrypt(resetUser.Password);
        return await _rep.AsUpdateable(user).UpdateColumns(u => u.Password).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除聚AI用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Delete")]
    public async Task Delete(DeleteAppUserInput input)
    {
        var entity = await _rep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        await _rep.FakeDeleteAsync(entity);   //假删除
    }

    /// <summary>
    /// 获取聚AI用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiDescriptionSettings(Name = "Detail")]
    public async Task<AppUser> Get([FromQuery] QueryByIdAppUserInput input)
    {
        return await _rep.GetFirstAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// 获取聚AI用户列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiDescriptionSettings(Name = "List")]
    public async Task<List<AppUserOutput>> List([FromQuery] AppUserInput input)
    {
        return await _rep.AsQueryable().Select<AppUserOutput>().ToListAsync();
    }

}

