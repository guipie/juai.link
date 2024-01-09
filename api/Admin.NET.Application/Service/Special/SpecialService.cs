using Admin.NET.Application.Const;
using System.Linq;

namespace Admin.NET.Application.JuAI;
/// <summary>
/// 专栏服务
/// </summary>
[ApiDescriptionSettings(ApplicationAppConst.GroupName, Order = 100)]
public class SpecialService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<Special> _rep;
    public SpecialService(SqlSugarRepository<Special> rep)
    {
        _rep = rep;
    }

    /// <summary>
    /// 分页查询专栏
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Page")]
    public async Task<SqlSugarPagedList<SpecialOutput>> Page(SpecialInput input)
    {
        var query = _rep.AsQueryable()
                    .WhereIF(!string.IsNullOrWhiteSpace(input.Title), u => u.Title.Contains(input.Title.Trim()))
                    .WhereIF(!string.IsNullOrWhiteSpace(input.Text), u => u.Text.Contains(input.Text.Trim()))
                     .Select(u => new SpecialOutput { });
        //.Mapper(c => c.CoverAttachment, c => c.Cover) 
        query = query.OrderBuilder(input);
        return await query.ToPagedListAsync(input.Page, input.PageSize);
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
        var list = ids.Select(m => new Special() { Id = m, Status = contentStatus }).ToList();
        return await _rep.AsUpdateable(list).UpdateColumns(m => m.Status).ExecuteCommandAsync();
    }
    /// <summary>
    /// 增加专栏
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Add")]
    public async Task Add(AddSpecialInput input)
    {
        var entity = input.Adapt<Special>();
        await _rep.InsertAsync(entity);
    }
    /// <summary>
    /// 删除专栏
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Delete")]
    public async Task Delete(DeleteSpecialInput input)
    {
        var entity = await _rep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        await _rep.JuAIFakeDeleteAsync(entity);   //假删除 
    }

    /// <summary>
    /// 更新专栏
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Update")]
    public async Task Update(UpdateSpecialInput input)
    {
        var entity = input.Adapt<Special>();
        await _rep.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取专栏
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiDescriptionSettings(Name = "Detail")]
    public async Task<Special> Get([FromQuery] QueryByIdSpecialInput input)
    {
        return await _rep.GetFirstAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// 获取专栏列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiDescriptionSettings(Name = "List")]
    public async Task<List<SpecialOutput>> List([FromQuery] SpecialInput input)
    {
        return await _rep.AsQueryable().Select<SpecialOutput>().ToListAsync();
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

