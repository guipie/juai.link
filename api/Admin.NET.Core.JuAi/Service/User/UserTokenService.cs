using Admin.NET.Core.JuAI.Entity;
using Furion.LinqBuilder;
using System.Linq.Expressions;

namespace Admin.NET.Core.JuAI.Service;
[ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 4)]
public class UserTokenService : AppBaseService, IScoped
{
    private readonly SqlSugarRepository<TokenRecord> _tokenRecord;
    private readonly SqlSugarRepository<AppUser> _appUser;
    private readonly UserManager _userManage;
    public UserTokenService(SqlSugarRepository<TokenRecord> tokenRecord, UserManager userManager, SqlSugarRepository<AppUser> appUser)
    {
        _tokenRecord = tokenRecord;
        _userManage = userManager;
        _appUser = appUser;
    }
    [NonAction]
    public async Task<bool> SetMinusUserToken(long num)
    {
        num = (num * 1.5).ToLong();
        var result = await _appUser.AsUpdateable().SetColumns(m => m.TokenNum == m.TokenNum - num)
                                                      .SetColumns(m => m.TokenLastUseDate == DateTime.Now)
                                                      .Where(m => m.Id == _userManage.UserId).ExecuteCommandAsync();
        return result > 0;
    }
    public long GetUserToken()
    {
        return _appUser.AsQueryable().Where(m => m.Id == _userManage.UserId).Select(m => m.TokenNum).First();
    }
    /// <summary>
    /// 获取token枚举类型
    /// </summary>
    /// <returns></returns>
    public List<DicItem> GetTokenTypes()
    {
        // 获取枚举的类型信息  
        Type enumType = typeof(TokenAddTypeEnum);

        // 获取枚举的名称数组和值数组  
        string[] enumNames = Enum.GetNames(enumType);
        Array enumValues = Enum.GetValues(enumType);
        var dic = new List<DicItem>();

        // 遍历枚举的名称和值数组  
        for (int i = 0; i < enumNames.Length; i++)
            dic.Add(new DicItem(enumNames[i], enumValues.GetValue(i).GetDescription()));
        return dic;
    }
    /// <summary>
    /// token记录
    /// </summary>
    /// <param name="lastTime"></param>
    /// <returns></returns>
    public async Task<object> GetTokenRecord(DateTime lastTime, TokenAddTypeEnum? type)
    {
        Expression<Func<TokenRecord, bool>> expression = m => m.CreateTime < lastTime && m.CreateUserId == _userManage.UserId;
        if (type != null) expression = expression.And(m => m.TokenAddType == type);
        var list = await _tokenRecord.AsQueryable().Where(expression).Take(10).OrderByDescending(m => m.CreateTime).ToListAsync();
        return list.Select(m => new
        {
            m.CreateTime,
            m.AddNum,
            TokenAddType = m.TokenAddType.GetDescription(),
            m.Extend,
            m.Id
        });
    }
    public async Task<int> GetTokenRecordCnt(DateTime lastTime, TokenAddTypeEnum? type)
    {
        Expression<Func<TokenRecord, bool>> expression = m => m.CreateTime < lastTime && m.CreateUserId == _userManage.UserId;
        if (type != null) expression = expression.And(m => m.TokenAddType == type);
        return await _tokenRecord.AsQueryable().Where(expression).Take(10).OrderByDescending(m => m.CreateTime).CountAsync();
    }
    /// <summary>
    /// 删除记录
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<bool> DeleteTokenRecord([Required] long id)
    {
        return await _tokenRecord.DeleteByIdAsync(id);
    }
}
