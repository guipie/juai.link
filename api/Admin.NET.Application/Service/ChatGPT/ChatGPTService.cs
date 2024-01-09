

using System.Linq;

namespace Admin.NET.Application.JuAI;
/// <summary>
/// AI会话服务
/// </summary>
[ApiDescriptionSettings(ApplicationAppConst.GroupName, Order = 100)]
public class ChatGPTService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<ChatGPT> _rep;
    public ChatGPTService(SqlSugarRepository<ChatGPT> rep)
    {
        _rep = rep;
    }

    /// <summary>
    /// 分页查询AI会话
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Page")]
    public async Task<SqlSugarPagedList<ChatGPTOutput>> Page(ChatGPTInput input)
    {
        var query = _rep.AsQueryable()
                    .WhereIF(!string.IsNullOrWhiteSpace(input.Question), u => u.Question.Contains(input.Question.Trim()))
                    .WhereIF(!string.IsNullOrWhiteSpace(input.Answer), u => u.Answer.Contains(input.Answer.Trim()))
                    .WhereIF(input.ReqNum > 0, u => u.ReqNum >= input.ReqNum)
                    .WhereIF(input.ResNum > 0, u => u.ResNum >= input.ResNum)
                    .WhereIF(!string.IsNullOrWhiteSpace(input.Model), u => u.Model == input.Model)

                    .Select(u => new ChatGPTOutput { Model = u.Model }, true)
;
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
    public async Task<bool> RecommendTop([FromBody] long[] ids, [FromQuery] ContentStatusEnum contentStatus = ContentStatusEnum.Passed)
    {
        return await _rep.UpdateAsync(m => new ChatGPT() { Status = contentStatus }, w => SqlFunc.ContainsArray(ids, w.Id));
    }
    /// <summary>
    /// 增加AI会话
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Add")]
    public async Task Add(AddChatGPTInput input)
    {
        var entity = input.Adapt<ChatGPT>();
        await _rep.InsertAsync(entity);
    }

    /// <summary>
    /// 删除AI会话
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Delete")]
    public async Task<int> Delete(DeleteChatGPTInput input)
    {
        var entity = await _rep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        return await _rep.JuAIFakeDeleteAsync(entity);   //假删除
    }

    /// <summary>
    /// 更新AI会话
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Update")]
    public async Task Update(UpdateChatGPTInput input)
    {
        var entity = input.Adapt<ChatGPT>();
        await _rep.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取AI会话
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiDescriptionSettings(Name = "Detail")]
    public async Task<ChatGPT> Get([FromQuery] QueryByIdChatGPTInput input)
    {
        return await _rep.GetFirstAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// 获取AI会话列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiDescriptionSettings(Name = "List")]
    public async Task<List<ChatGPTOutput>> List([FromQuery] ChatGPTInput input)
    {
        return await _rep.AsQueryable().Select<ChatGPTOutput>().ToListAsync();
    }

    /// <summary>
    /// 获取模型列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "ChatModelDropdown"), HttpGet]
    public async Task<dynamic> ChatModelDropdown()
    {
        return await _rep.Context.Queryable<ChatModel>()
                .Select(u => new
                {
                    Label = u.Name,
                    Value = u.Id
                }
                ).ToListAsync();
    }




}

