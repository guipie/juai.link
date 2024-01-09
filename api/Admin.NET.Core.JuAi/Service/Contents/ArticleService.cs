using Furion.DatabaseAccessor;
using Furion.LinqBuilder;
using System.Linq.Expressions;

namespace Admin.NET.Core.JuAI.Service;
[ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 200)]
public class ArticleService : AppBaseService
{
    private readonly ICache _cache;
    private readonly SqlSugarRepository<SysFile> _sysFileRes;
    private readonly SqlSugarRepository<Article> _articleRes;
    private readonly SysFileService _fileService;
    private readonly JuFileService _juFileService;
    private readonly UserManager _userManage;

    public ArticleService(ICache cache,
        SqlSugarRepository<SysFile> sysFileRes,
        SysFileService sysFileService,
        SqlSugarRepository<Article> articleRes,
        UserManager userManage,
        JuFileService juFileService)
    {
        _cache = cache;
        _sysFileRes = sysFileRes;
        _fileService = sysFileService;
        _articleRes = articleRes;
        _userManage = userManage;
        _juFileService = juFileService;
    }
    [DisplayName("新增修改文章"), UnitOfWork]
    public async Task<long> Post(ArticleInput articleInput)
    {
        if (articleInput.Id > 0)
        {
            var existModel = await _articleRes.GetByIdAsync(articleInput.Id);
            if (existModel == null || existModel.CreateUserId != _userManage.UserId) throw Oops.Oh("您无权操作");
        }
        ContentStatusEnum status = _userManage.SuperAdmin ? ContentStatusEnum.Passed : ContentStatusEnum.Pending;
        var config = new TypeAdapterConfig().ForType<ArticleInput, Article>()
                                                    .Map(dest => dest.Status, src => status);
        var articleNew = articleInput.Adapt<Article>(config.Config);
        await _articleRes.InsertOrUpdateAsync(articleNew);
        await _juFileService.PostFileRelationId(articleInput.FileIds, articleNew.Id, FileRelationEnum.Article);
        return articleNew.Id;
    }
    [DisplayName("删除文章")]
    public async Task<bool> Delete([FromRoute] long id)
    {
        var existModel = await _articleRes.GetByIdAsync(id);
        if (existModel == null || existModel.CreateUserId != _userManage.UserId) throw Oops.Oh("您无权操作");
        return (await _articleRes.JuAIFakeDeleteAsync(existModel)) > 0;
    }
    public async Task<List<ArticleAppOutput>> GetMy([FromQuery] long? minId, [FromQuery] int? size)
    {
        Expression<Func<Article, bool>> whereExpression = m => m.CreateUserId == _userManage.UserId;
        if (minId.HasValue && minId > 0) whereExpression = whereExpression.And(m => m.Id < minId);
        var list = await _articleRes.AsQueryable().Where(whereExpression).OrderByDescending(m => m.Id).Take(size ?? 10).ToListAsync();
        return list.Adapt<List<ArticleAppOutput>>();
    }

    [AllowAnonymous]
    public async Task<List<ArticleAppOutput>> GetTop()
    {
        Expression<Func<Article, bool>> whereExpression = m => m.Status == ContentStatusEnum.Top;
        var list = await _articleRes.AsQueryable().Includes(x => x.CreateUser)
                                                .Where(whereExpression).OrderByDescending(m => m.Id)
                                                .Take(20).ToListAsync();
        return list.Adapt<List<ArticleAppOutput>>();
    }
    [AllowAnonymous]
    public async Task<List<ArticleAppOutput>> GetNewest([FromQuery] long? minId, [FromQuery] int size = 15)
    {
        Expression<Func<Article, bool>> whereExpression = m => m.Status > 0;
        if (_userManage != null && _userManage.UserId > 0) whereExpression = whereExpression.Or(m => m.CreateUserId == _userManage.UserId);
        if (minId.HasValue && minId > 0) whereExpression = whereExpression.And(m => m.Id < minId);
        var list = await _articleRes.AsQueryable().Includes(x => x.CreateUser)
                                                .Where(whereExpression).OrderByDescending(m => m.Id)
                                                .Take(size).ToListAsync();
        return list.Adapt<List<ArticleAppOutput>>();
    }
    /// <summary>
    /// 推荐，右侧边栏
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    public async Task<List<ArticleSiderOutput>> GetRecommend()
    {
        Expression<Func<Article, bool>> whereExpression = m => m.Status == ContentStatusEnum.Recommend;
        var list = await _articleRes.AsQueryable().Where(whereExpression)
                                                        .OrderByDescending(m => new { m.CommentCount, m.ViewCount })
                                                        .Take(10).ToListAsync();
        if (list.Count > 0)
            return list.Adapt<List<ArticleSiderOutput>>();
        return (await GetNewest(null)).Select(m => new ArticleSiderOutput(m.Id, m.Title)).Take(10).ToList();
    }
    [AllowAnonymous]
    public async Task<ArticleOutputHtml> GetDetail([Required, FromRoute] long id)
    {
        var detail = await _articleRes.GetByIdAsync(id);
        _ = Task.Run(() =>
        {
            string onlyKey = App.HttpContext.GetRemoteIpAddressToIPv4() ?? "no_ip";
            var datetNow = DateTime.Now;
            if (_cache.ContainsKey(onlyKey)) return;
            _cache.Set(onlyKey, datetNow, (datetNow.AddDays(1).Date - datetNow).TotalSeconds.ToInt());
            detail.ViewCount++;
            _articleRes.AsUpdateable(detail).UpdateColumns(m => m.ViewCount).ExecuteCommandAsync();
        });
        return detail.Adapt<ArticleOutputHtml>();
    }
}
