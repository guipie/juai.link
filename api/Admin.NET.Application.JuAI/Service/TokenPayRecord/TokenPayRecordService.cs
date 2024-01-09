using Admin.NET.Application.Const;

namespace Admin.NET.Application.JuAI;
/// <summary>
/// 充值记录服务
/// </summary>
[ApiDescriptionSettings(ApplicationAppConst.GroupName, Order = 100)]
public class TokenPayRecordService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<TokenPayRecord> _rep;
    public TokenPayRecordService(SqlSugarRepository<TokenPayRecord> rep)
    {
        _rep = rep;
    }

    /// <summary>
    /// 分页查询充值记录
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Page")]
    public async Task<SqlSugarPagedList<TokenPayRecordOutput>> Page(TokenPayRecordInput input)
    {
        var query = _rep.AsQueryable()
                    .WhereIF(!string.IsNullOrWhiteSpace(input.Orderno), u => u.OrderNo.Contains(input.Orderno.Trim()))
                    .WhereIF(input.Bepaynum > 0, u => u.BePayNum >= input.Bepaynum)
                    .WhereIF(input.PayedNum > 0, u => u.PayedNum >= input.PayedNum)

                    .Select<TokenPayRecordOutput>()
;
        if (input.BepaydateRange != null && input.BepaydateRange.Count > 0)
        {
            DateTime? start = input.BepaydateRange[0];
            query = query.WhereIF(start.HasValue, u => u.Bepaydate > start);
            if (input.BepaydateRange.Count > 1 && input.BepaydateRange[1].HasValue)
            {
                var end = input.BepaydateRange[1].Value.AddDays(1);
                query = query.Where(u => u.Bepaydate < end);
            }
        }
        if (input.PayedDateRange != null && input.PayedDateRange.Count > 0)
        {
            DateTime? start = input.PayedDateRange[0];
            query = query.WhereIF(start.HasValue, u => u.PayedDate > start);
            if (input.PayedDateRange.Count > 1 && input.PayedDateRange[1].HasValue)
            {
                var end = input.PayedDateRange[1].Value.AddDays(1);
                query = query.Where(u => u.PayedDate < end);
            }
        }
        query = query.OrderBuilder(input);
        return await query.ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 增加充值记录
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Add")]
    public async Task Add(AddTokenPayRecordInput input)
    {
        var entity = input.Adapt<TokenPayRecord>();
        await _rep.InsertAsync(entity);
    }

    /// <summary>
    /// 删除充值记录
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Delete")]
    public async Task<int> Delete(DeleteTokenPayRecordInput input)
    {
        var entity = await _rep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        return await _rep.JuAIFakeDeleteAsync(entity);   //假删除
    }

    /// <summary>
    /// 更新充值记录
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Update")]
    public async Task Update(UpdateTokenPayRecordInput input)
    {
        var entity = input.Adapt<TokenPayRecord>();
        await _rep.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取充值记录
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiDescriptionSettings(Name = "Detail")]
    public async Task<TokenPayRecord> Get([FromQuery] QueryByIdTokenPayRecordInput input)
    {
        return await _rep.GetFirstAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// 获取充值记录列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiDescriptionSettings(Name = "List")]
    public async Task<List<TokenPayRecordOutput>> List([FromQuery] TokenPayRecordInput input)
    {
        return await _rep.AsQueryable().Select<TokenPayRecordOutput>().ToListAsync();
    }





}

