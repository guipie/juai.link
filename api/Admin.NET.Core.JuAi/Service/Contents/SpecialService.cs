using Furion.DatabaseAccessor;
using Furion.LinqBuilder;
using System.Linq.Expressions;

namespace Admin.NET.Core.JuAI.Service;
[ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 200)]
public class SpecialService : AppBaseService
{
    static string configId = App.GetOptions<DbConnectionOptions>().ConnectionConfigs[0].ConfigId.ToString() ?? "";
    private readonly ICache _cache;
    private readonly SqlSugarRepository<SysFile> _sysFileRes;
    private readonly SqlSugarRepository<Article> _articleRes;
    private readonly SqlSugarRepository<Special> _specialRes;
    private readonly SysFileService _fileService;
    private readonly JuFileService _juFileService;
    private readonly UserManager _userManage;

    public SpecialService(ICache cache,
        SqlSugarRepository<SysFile> sysFileRes,
        SysFileService sysFileService,
        SqlSugarRepository<Article> articleRes,
        UserManager userManage,
        SqlSugarRepository<Special> specialRes, JuFileService juFileService
        )
    {
        _cache = cache;
        _sysFileRes = sysFileRes;
        _fileService = sysFileService;
        _articleRes = articleRes;
        _userManage = userManage;
        _specialRes = specialRes;
        _juFileService = juFileService;
    }

    [DisplayName("新增修改专栏"), UnitOfWork]
    public async Task<long> Post([FromBody] SpecialInput input)
    {
        if (input.Id > 0)
        {
            var existModel = await _specialRes.GetByIdAsync(input.Id);
            if (existModel == null || existModel.CreateUserId != _userManage.UserId) throw Oops.Oh("您无权操作");
        }
        var addModel = input.Adapt<Special>();
        addModel.Status = _userManage.SuperAdmin ? ContentStatusEnum.Passed : ContentStatusEnum.Pending;
        var special = await _specialRes.InsertOrUpdateAsync(addModel);
        if (special)
            await _juFileService.PostFileRelationId([input.CoverFileId], addModel.Id, FileRelationEnum.Special);
        return addModel.Id;
    }

    [DisplayName("删除专栏")]
    public async Task<bool> Delete([FromRoute] long id)
    {
        var existModel = await _specialRes.GetByIdAsync(id);
        if (existModel == null || existModel.CreateUserId != _userManage.UserId) throw Oops.Oh("您无权操作");
        var articleCnt = await _articleRes.IsAnyAsync(m => m.SpecialId == id);
        if (articleCnt) throw Oops.Oh("请将您的专栏文章删除或转移后再删除此专栏.");
        return (await _specialRes.JuAIFakeDeleteAsync(existModel)) > 0;
    }
    /// <summary>
    /// 专栏详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [AllowAnonymous]
    public async Task<SpecialOutput> GetDetail([FromRoute] long id)
    {
        var model = await _specialRes.GetByIdAsync(id);
        return model.Adapt<SpecialOutput>();
    }
    /// <summary>
    /// 我的专栏
    /// </summary>
    /// <returns></returns>
    public async Task<IList<SpecialOutput>> GetMy([FromQuery] long? minId, [FromQuery] int? size)
    {
        return await _specialRes.AsQueryable().Where(m => m.CreateUserId == _userManage.UserId)
                                              .WhereIF(minId > 0, m => m.Id < minId)
                                              .Take(size ?? 10)
                                              .OrderByDescending(m => m.CreateTime)
                                              .Select(m => new SpecialOutput() { }, true)
                                              .ToListAsync();
    }
    /// <summary>
    /// 最新推荐(置顶在最上面)列表分页
    /// </summary>
    /// <param name="minId"></param>
    /// <returns></returns>
    [AllowAnonymous]
    public async Task<IList<SpecialOutput>> GetRecommand([FromQuery] int page = 1, [FromQuery] int size = 15)
    {
        Expression<Func<Special, bool>> whereExpression = m => m.Status > 0;
        if (_userManage != null && _userManage.UserId > 0) whereExpression = whereExpression.Or(m => m.CreateUserId == _userManage.UserId);
        return await _specialRes.AsQueryable().Where(whereExpression).OrderByDescending(m => m.Status).OrderByDescending(m => m.UpdateTime)
                                              .Select(m => new SpecialOutput() { }, true)
                                              .ToOffsetPageAsync(page, size);
    }
    /// <summary>
    /// 最新专栏列表分页
    /// </summary>
    /// <param name="minId"></param>
    /// <returns></returns>
    [AllowAnonymous]
    public async Task<IList<SpecialOutput>> GetNewest([FromQuery] long? minId)
    {
        Expression<Func<Special, bool>> whereExpression = m => m.Status > 0;
        if (_userManage != null && _userManage.UserId > 0) whereExpression = whereExpression.Or(m => m.CreateUserId == _userManage.UserId);
        if (minId.HasValue && minId > 0) whereExpression = whereExpression.And(m => m.Id < minId);
        return await _specialRes.AsQueryable().Where(whereExpression).OrderByDescending(m => m.Id)
                                              .Select(m => new SpecialOutput() { }, true)
                                              .Take(10).ToListAsync();
    }
    /// <summary>
    /// 获取专栏文章
    /// </summary>
    /// <param name="specialId"></param>
    /// <returns></returns>
    [AllowAnonymous]
    public async Task<IList<SpecialArticleOutput>> GetArticles([FromRoute] long specialId, [FromQuery] long? minId, [FromQuery] int size)
    {
        Expression<Func<Article, bool>> expression = m => m.SpecialId == specialId && m.Status > 0;
        if (minId.HasValue && minId > 0) expression = expression.And(m => m.Id < minId);
        return await _articleRes.AsQueryable().CrossQuery(typeof(SysFile), configId)
                                .Includes(x => x.Files)
                                .Where(expression).OrderByDescending(m => m.Id).Take(size)
                                .Select(m => new SpecialArticleOutput() { Files = m.Files!.Take(9).ToList() }, true)
                                .ToListAsync();
    }

}
