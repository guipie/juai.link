using Furion.DatabaseAccessor;
using Microsoft.AspNetCore.Identity;

namespace Admin.NET.Core.JuAI.Service;

[AllowAnonymous]
[ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 15)]
public class UserService : AppBaseService
{

    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<AppUser> _userRep;
    private readonly SqlSugarRepository<TokenRecord> _tokenRecordRep;
    public UserService(UserManager userManager, SqlSugarRepository<AppUser> userRep, SqlSugarRepository<TokenRecord> tokenRecord)
    {
        _userRep = userRep;
        _userManager = userManager;
        _tokenRecordRep = tokenRecord;
    }

    /// <summary>
    /// 今日是否签到
    /// </summary>
    /// <returns></returns>
    public async Task<bool> GetIsQianDaoToday()
    {
        return await _tokenRecordRep.IsAnyAsync(m => m.CreateUserId == _userManager.UserId && SqlFunc.DateIsSame(m.CreateTime, DateTime.Now) && m.TokenAddType == TokenAddTypeEnum.QianDao);
    }
    /// <summary>
    /// 签到
    /// </summary>
    /// <returns></returns>
    [UnitOfWork]
    public async Task<bool> PostQianDaoToday()
    {
        var isQiandao = await _tokenRecordRep.IsAnyAsync(m => m.TokenAddType == TokenAddTypeEnum.QianDao && m.CreateUserId == _userManager.UserId && SqlFunc.DateIsSame(m.CreateTime, DateTime.Now));
        if (isQiandao) throw Oops.Bah("今日已签到");
        await _tokenRecordRep.InsertAsync(new TokenRecord { AddNum = 10000, TokenAddType = TokenAddTypeEnum.QianDao });
        return _userRep.AsUpdateable().SetColumns(m => m.TokenNum == m.TokenNum + 10000)
                       .Where(m => m.Id == _userManager.UserId).ExecuteCommand() > 0;
    }

    [DisplayName("设置获取邀请码")]
    public async Task<string> GetInviteCode()
    {
        var vcode = _userRep.GetFirst(m => m.Id == _userManager.UserId).Vcode;
        if (vcode == null)
        {
            vcode = StringExtension.CreateRandStrCode(4, 10000);
            if (await _userRep.IsAnyAsync(m => m.Vcode == vcode))
                throw Oops.Bah("邀请码生成错误，请刷新重新生成");
            await _userRep.UpdateAsync(m => new AppUser() { Vcode = vcode }, m => m.Id == _userManager.UserId);
        }
        return vcode;
    }
    [DisplayName("获取我邀请的人")]
    public async Task<SqlSugarPagedList<TokenRecord>> GetInviteUsers(int page, int size)
    {
        return await _tokenRecordRep.AsQueryable()
                   .Where(m => m.TokenAddType == TokenAddTypeEnum.Invite && m.CreateUserId == _userManager.UserId)
                   .ToPagedListAsync(page, size);
    }
    [DisplayName("活跃用户")]
    public IList<UserInfoOutput> GetActiveUsers()
    {
        return _userRep.AsQueryable()
                        .Where(m => m.Status == StatusEnum.Enable)
                        .Take(10)
                        .OrderByDescending(m => m.LastLoginTime)
                        .Select(m => new UserInfoOutput() { Id = m.Id, Avatar = m.Avatar, NickName = m.NickName }).ToList();
    }
}
