using Admin.NET.Application.Const;
using Admin.NET.Application.Service.Home.Dto;
using NewLife.Caching;
using System.Linq;

namespace Admin.NET.Application.JuAI;
/// <summary>
/// 聚AI内容服务
/// </summary>
[ApiDescriptionSettings(ApplicationAppConst.GroupName, Order = 100)]
public class StatisticsService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<Article> _articleRep;
    private readonly SqlSugarRepository<Special> _specialRep;
    private readonly SqlSugarRepository<AppUser> _userRep;
    private readonly SqlSugarRepository<ChatGPT> _chagptRep;
    private readonly SqlSugarRepository<ChatGPTImage> _gptImageRep;
    private readonly SqlSugarRepository<MidjourneyTask> _midImageRep;
    private readonly SqlSugarRepository<TokenPayRecord> _payedRes;
    private readonly ICache _cache;
    public StatisticsService(SqlSugarRepository<Article> articleRep, SqlSugarRepository<AppUser> userRep, SqlSugarRepository<ChatGPT> chagptRep,
        SqlSugarRepository<ChatGPTImage> gptImageRep, SqlSugarRepository<MidjourneyTask> midImageRep, SqlSugarRepository<Special> specialRep,
        SqlSugarRepository<TokenPayRecord> payedRes, ICache cache)
    {
        _articleRep = articleRep;
        _userRep = userRep;
        _chagptRep = chagptRep;
        _gptImageRep = gptImageRep;
        _midImageRep = midImageRep;
        _specialRep = specialRep;
        _payedRes = payedRes;
        _cache = cache;
    }

    /// <summary>
    /// 首页统计用户
    /// </summary> 
    /// <returns></returns>
    public async Task<object> GetHomeUser()
    {
        Dictionary<string, int> obj = _cache.Get<Dictionary<string, int>>(AppCacheConst.Statistics_Home_User_Key);
        if (obj.IsNullOrEmpty())
            obj = new()
            {
                ["总用户"] = await _userRep.CountAsync(m => true),
                ["日活用户"] = await _userRep.CountAsync(m => SqlFunc.DateIsSame(m.LastLoginTime, DateTime.Now)),
                ["今日注册用户"] = await _userRep.CountAsync(m => SqlFunc.DateIsSame(m.CreateTime, DateTime.Now)),
                ["今日使用AI用户"] = await _userRep.CountAsync(m => SqlFunc.DateIsSame(m.TokenLastUseDate, DateTime.Now)),
            };
        _cache.Set(AppCacheConst.Statistics_Home_User_Key, obj, 3600);
        return obj.Select(m => new { m.Key, m.Value }).ToList();
    }

    /// <summary>
    /// 首页统计内容
    /// </summary> 
    /// <returns></returns>
    public async Task<object> GetHomeContent()
    {
        Dictionary<string, int> obj = _cache.Get<Dictionary<string, int>>(AppCacheConst.Statistics_Home_Content_Key);
        if (obj.IsNullOrEmpty())
            obj = new()
            {
                ["总文章"] = await _articleRep.CountAsync(m => true),
                ["今日新增文章"] = await _articleRep.CountAsync(m => SqlFunc.DateIsSame(m.CreateTime, DateTime.Now)),
                ["总专栏"] = await _specialRep.CountAsync(m => true),
                ["今日新增专栏"] = await _specialRep.CountAsync(m => SqlFunc.DateIsSame(m.CreateTime, DateTime.Now)),
            };
        _cache.Set(AppCacheConst.Statistics_Home_Content_Key, obj, 3600);
        return obj.Select(m => new { m.Key, m.Value }).ToList();
    }
    /// <summary>
    /// 首页统计内容
    /// </summary> 
    /// <returns></returns>
    public async Task<object> GetHomeAI()
    {
        Dictionary<string, int> obj = _cache.Get<Dictionary<string, int>>(AppCacheConst.Statistics_Home_AI_Key);
        if (obj.IsNullOrEmpty())
            obj = new()
            {
                ["总会话"] = await _chagptRep.CountAsync(m => true),
                ["今日新增会话"] = await _chagptRep.CountAsync(m => SqlFunc.DateIsSame(m.CreateTime, DateTime.Now)),
                ["总绘画(GPT)"] = await _gptImageRep.CountAsync(m => true),
                ["今日新增绘画(GPT)"] = await _gptImageRep.CountAsync(m => SqlFunc.DateIsSame(m.CreateTime, DateTime.Now)),
                ["总绘画(Mid)"] = await _midImageRep.CountAsync(m => true),
                ["今日新增绘画(Mid)"] = await _midImageRep.CountAsync(m => SqlFunc.DateIsSame(m.CreateTime, DateTime.Now)),
            };
        _cache.Set(AppCacheConst.Statistics_Home_AI_Key, obj, 3600);
        return obj.Select(m => new { m.Key, m.Value }).ToList();
    }
    /// <summary>
    /// 首页统计付费
    /// </summary> 
    /// <returns></returns>
    public async Task<object> GetHomePay()
    {
        Dictionary<string, double?> obj = _cache.Get<Dictionary<string, double?>>(AppCacheConst.Statistics_Home_Pay_Key);
        if (obj.IsNullOrEmpty())
            obj = new Dictionary<string, double?>
            {
                ["付费总人数"] = await _payedRes.AsQueryable().Where(m => m.PayedRmb > 0).Select(m => m.CreateUserId).Distinct().CountAsync(),
                ["今日付费人数"] = await _payedRes.AsQueryable().Where(m => m.PayedRmb > 0 && SqlFunc.DateIsSame(m.CreateTime, DateTime.Now)).Select(m => m.CreateUserId).Distinct().CountAsync(),
                ["付费总金额"] = await _payedRes.AsQueryable().SumAsync(m => m.PayedRmb),
                ["今日付费金额"] = await _payedRes.AsQueryable().Where(m => SqlFunc.DateIsSame(m.CreateTime, DateTime.Now)).SumAsync(m => m.PayedRmb)
            };
        _cache.Set(AppCacheConst.Statistics_Home_Pay_Key, obj, 3600);
        return obj.Select(m => new { m.Key, m.Value }).ToList();
    }
}

