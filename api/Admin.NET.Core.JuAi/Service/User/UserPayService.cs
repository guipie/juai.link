using Admin.NET.Core.JuAI.Entity;
using Furion.DatabaseAccessor;
using Furion.Logging;
using Microsoft.Extensions.Logging;

namespace Admin.NET.Core.JuAI.Service;
[ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 4)]
public class UserPayService : AppBaseService, IScoped
{
    private readonly SqlSugarRepository<TokenPayRecord> _tokenPay;
    private readonly SqlSugarRepository<TokenRecord> _tokenRecord;
    private readonly SqlSugarRepository<AppUser> _appUser;
    private readonly UserManager _userManage;
    public UserPayService(SqlSugarRepository<TokenPayRecord> tokenPay, SqlSugarRepository<TokenRecord> tokenRecord, UserManager userManager, SqlSugarRepository<AppUser> appUser)
    {
        _tokenPay = tokenPay;
        _tokenRecord = tokenRecord;
        _userManage = userManager;
        _appUser = appUser;
    }
    /// <summary>
    /// 充值记录
    /// </summary>
    /// <param name="lastTime"></param>
    /// <returns></returns>
    public async Task<object> GetPayRecord(DateTime lastTime)
    {
        var list = await _tokenPay.AsQueryable().Where(m => m.CreateTime < lastTime && m.CreateUserId == _userManage.UserId).IgnoreColumns(m => m.TenantId).ToListAsync();
        return list.OrderByDescending(m => m.CreateTime);
    }
    /// <summary>
    /// 充值前验证
    /// </summary>
    /// <param name="payInfo"></param>
    /// <returns></returns>
    [HttpPost("pay/before")]
    public bool PayBefore([FromBody] JsonData payInfo)
    {
        var decryptStr = CryptogramUtilExtend.DecryptByAES(payInfo.Data, "juai.link", "juai.link");
        var input = JSON.Deserialize<UserPayInfoInput>(decryptStr);
        return _tokenPay.AsInsertable(new TokenPayRecord()
        {
            BePayDate = input.Date,
            BePayNum = input.Num,
            BePayRmb = input.RMB,
            OrderNo = input.OrderNo
        }).ExecuteCommand() > 0;
    }
    [HttpGet("pay/success"), AllowAnonymous, UnitOfWork]
    public async Task<bool> PaySuccess([FromQuery] string param)
    {
        try
        {
            param = CryptogramUtilExtend.Base64Decode(param);
            var decryptStr = CryptogramUtilExtend.DecryptByAES(param, "juai.link", "juai.link");
            var input = JSON.Deserialize<UserPayInfoInput>(decryptStr);
            if (input != null)
            {
                var payedRecord = _tokenPay.GetFirst(m => m.OrderNo == input.OrderNo);
                if (payedRecord == null || payedRecord.PayedNum > 0 || payedRecord.PayedRmb > 0 || !payedRecord.PayedDate.IsNullOrEmpty())
                    throw new Exception("支付出现异常，尽快处理," + payedRecord.ToJson());
                await _tokenPay.UpdateAsync(m =>
                new TokenPayRecord()
                {
                    PayedDate = DateTime.Now,
                    PayedNum = input.Num,
                    PayedRmb = input.RMB
                }, m => m.OrderNo == input.OrderNo);
                await _tokenRecord.InsertAsync(new TokenRecord()
                {
                    AddNum = input.Num,
                    TokenAddType = TokenAddTypeEnum.Pay,
                    CreateUserId = payedRecord.CreateUserId,
                    TenantId = payedRecord.TenantId
                });
                return _appUser.AsUpdateable()
                               .SetColumns(m => m.TokenNum == m.TokenNum + input.Num)
                               .SetColumns(m => m.PayType == (input.RMB == 9 ? PayTypeEnum.MonthPayed : PayTypeEnum.CustomPayed))
                               .SetColumns(m => m.PayDateTime == DateTime.Now)
                               .Where(m => m.Id == payedRecord.CreateUserId).ExecuteCommand() > 0;
            }
            return false;
        }
        catch (Exception ex)
        {
            Log.CreateLogger<UserTokenService>().LogError("支付成功后出错了" + ex.Message, ex.StackTrace);
            return false;
        }
    }
}
