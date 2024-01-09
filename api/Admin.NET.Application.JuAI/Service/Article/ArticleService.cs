 

namespace Admin.NET.Application.JuAI;
/// <summary>
/// 聚AI内容服务
/// </summary>
[ApiDescriptionSettings(ApplicationAppConst.GroupName, Order = 100)]
public class ArticleService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<Article> _rep;
    public ArticleService(SqlSugarRepository<Article> rep)
    {
        _rep = rep;
    }

    /// <summary>
    /// 分页查询聚AI内容
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Page")]
    public async Task<SqlSugarPagedList<ArticleOutput>> Page(ArticleInput input)
    {
        var query = _rep.AsQueryable()
                    .WhereIF(!string.IsNullOrWhiteSpace(input.Title), u => u.Title.Contains(input.Title.Trim()))
                    .WhereIF(!string.IsNullOrWhiteSpace(input.Html), u => u.Html.Contains(input.Html.Trim()))
                    .WhereIF(!string.IsNullOrWhiteSpace(input.Text), u => u.Text.Contains(input.Text.Trim()))
                    .WhereIF(input.SpecialId > 0, u => u.SpecialId == input.SpecialId)

                    .Select(u => new ArticleOutput
                    {
                        Id = u.Id,
                        Title = u.Title,
                        Html = u.Html,
                        Text = u.Text,
                        Cover = u.Cover,
                        ViewCount = u.ViewCount,
                        CommentCount = u.CommentCount,
                        LikeCount = u.LikeCount,
                        Status = u.Status,
                        SpecialId = u.SpecialId,
                        SpecialName = u.SpecialName,
                    })
//.Mapper(c => c.CoverAttachment, c => c.Cover)
;
        query = query.OrderBuilder(input);
        return await query.ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 增加聚AI内容
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Add")]
    public async Task Add(AddArticleInput input)
    {
        var entity = input.Adapt<Article>();
        await _rep.InsertAsync(entity);
    }

    /// <summary>
    /// 推荐置顶
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="contentStatus"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "recommendTop")]
    public async Task<int> RecommendTop([FromBody] long[] ids, [FromQuery] ContentStatusEnum contentStatus = ContentStatusEnum.Passed)
    {
        {
            List<Article> list = ids.Select(m => new Article() { Id = m, Status = contentStatus }).ToList();
            return await _rep.AsUpdateable(list).UpdateColumns(m => m.Status).ExecuteCommandAsync();
        }
    }
    /// <summary>
    /// 删除聚AI内容
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Delete")]
    public async Task Delete(DeleteArticleInput input)
    {
        var entity = await _rep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        await _rep.JuAIFakeDeleteAsync(entity);   //假删除
    }

    /// <summary>
    /// 更新聚AI内容
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Update")]
    public async Task Update(UpdateArticleInput input)
    {
        var entity = input.Adapt<Article>();
        await _rep.AsUpdateable(entity).IgnoreColumns(m => m.Html).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取聚AI内容
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiDescriptionSettings(Name = "Detail")]
    public async Task<Article> Get([FromQuery] QueryByIdArticleInput input)
    {
        return await _rep.GetFirstAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// 获取聚AI内容列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiDescriptionSettings(Name = "List")]
    public async Task<List<ArticleOutput>> List([FromQuery] ArticleInput input)
    {
        return await _rep.AsQueryable().Select<ArticleOutput>().ToListAsync();
    }

    /// <summary>
    /// 获取专栏列表
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "SpecialDropdown"), HttpGet]
    public async Task<dynamic> SpecialDropdown()
    {
        return await _rep.Context.Queryable<Special>()
                .Select(u => new
                {
                    Label = u.Title,
                    Value = u.Id
                }
                ).ToListAsync();
    }

    /// <summary>
    /// 上传封面
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "UploadCover"), HttpPost]
    public async Task<FileOutput> UploadCover([Required] IFormFile file)
    {
        var service = App.GetService<SysFileService>();
        return await service.UploadFile(file, "upload/Cover");
    }



}

