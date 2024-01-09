using Admin.NET.Application.Const;
using Admin.NET.Core.JuAI.Service;
using Admin.NET.Core;
using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;
using System.Linq;
using Microsoft.Extensions.Options;
using Furion.Logging;
using Yitter.IdGenerator;

namespace Admin.NET.Application.JuAI;
/// <summary>
/// Mid绘画服务
/// </summary>
[ApiDescriptionSettings(ApplicationAppConst.GroupName, Order = 100)]
public class MidjourneyNotifyService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<MidjourneyNotify> _rep;
    public MidjourneyNotifyService(SqlSugarRepository<MidjourneyNotify> rep)
    {
        _rep = rep;
    }

    /// <summary>
    /// 分页查询Mid绘画
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Page")]
    public async Task<SqlSugarPagedList<MidjourneyNotifyOutput>> Page(MidjourneyNotifyInput input)
    {
        var query = _rep.AsQueryable().LeftJoin<MidjourneyTask>((n, t) => n.Id == t.Id)
                    .Select((n, t) => new MidjourneyNotifyOutput
                    {
                        Prompt = t.Prompt ?? n.Prompt,
                        SubmitTime = n.SubmitTime,
                    }, true).MergeTable()
                    .WhereIF(!string.IsNullOrWhiteSpace(input.Action), m => m.Action.Contains(input.Action.Trim()))
                    .WhereIF(!string.IsNullOrWhiteSpace(input.Prompt), m => m.Prompt.Contains(input.Prompt.Trim()))
                    .WhereIF(!string.IsNullOrWhiteSpace(input.PromptEn), m => m.PromptEn.Contains(input.PromptEn.Trim()))
                    .WhereIF(input.SubmitTime > 0, m => m.SubmitTime == input.SubmitTime);
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
    public async Task<bool> RecommendTop([FromBody] string[] ids, [FromQuery] ContentStatusEnum contentStatus = ContentStatusEnum.Passed)
    {
        return await _rep.UpdateAsync(m => new MidjourneyNotify() { ContentStatus = contentStatus }, w => ids.Contains(w.Id));
    }
    /// <summary>
    /// 增加Mid绘画
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Add")]
    public async Task Add(AddMidjourneyNotifyInput input)
    {
        var entity = input.Adapt<MidjourneyNotify>();
        entity.Action = "IMAGINE";
        entity.Status = "SUCCESS";
        entity.SubmitTime = DateTimeUtil.ToUnixTimestampByMilliseconds(DateTime.Now);
        entity.Id = DateTimeUtil.ToUnixTimestampByMilliseconds(DateTime.Now).ToString();
        await _rep.InsertAsync(entity);
    }

    /// <summary>
    /// 删除Mid绘画
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Delete")]
    public async Task Delete(DeleteMidjourneyNotifyInput input)
    {
        MidjourneyNotify entity = await _rep.GetFirstAsync(u => u.Id == input.Id.ToString()) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        await _rep.UpdateAsync(m => new MidjourneyNotify() { IsDelete = true }, m => m.Id == entity.Id);   //假删除
    }

    /// <summary>
    /// 更新Mid绘画
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Update")]
    public async Task Update(UpdateMidjourneyNotifyInput input)
    {
        var entity = input.Adapt<MidjourneyNotify>();
        await _rep.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取Mid绘画
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiDescriptionSettings(Name = "Detail")]
    public async Task<MidjourneyNotify> Get([FromQuery] QueryByIdMidjourneyNotifyInput input)
    {
        return await _rep.GetFirstAsync(u => u.Id == input.Id.ToString());
    }

    /// <summary>
    /// 获取Mid绘画列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiDescriptionSettings(Name = "List")]
    public async Task<List<MidjourneyNotifyOutput>> List([FromQuery] MidjourneyNotifyInput input)
    {
        return await _rep.AsQueryable().Select<MidjourneyNotifyOutput>().ToListAsync();
    }


    /// <summary>
    /// 上传云地址
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "UploadOssUrl"), HttpPost]
    public async Task<FileOutput> UploadOssUrl([Required] IFormFile file)
    {
        var service = App.GetService<SysFileService>();
        return await service.UploadFile(file, "upload/OssUrl");
    }


}

