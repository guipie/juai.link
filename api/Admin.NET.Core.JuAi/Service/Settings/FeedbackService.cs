
using Admin.NET.Core.JuAI.Service.Settings;
using Furion.DatabaseAccessor;
using Yitter.IdGenerator;

namespace Admin.NET.Core.JuAI.Service;
[ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 200)]
public class FeedbackService : AppBaseService
{
    private readonly SqlSugarRepository<Feedback> _feedbackRep;
    private readonly SqlSugarRepository<SysFile> _sysFileRes;
    private readonly ICache _cache;
    private readonly JuFileService _juFileService;

    public FeedbackService(SqlSugarRepository<Feedback> feedbackRep, ICache cache, JuFileService juFileService, SqlSugarRepository<SysFile> sysFileRes)
    {
        _cache = cache;
        _feedbackRep = feedbackRep;
        _juFileService = juFileService;
        _sysFileRes = sysFileRes;
    }

    [DisplayName("反馈数据"), UnitOfWork]
    public async Task<bool> PostFeedback([FromBody] FeedbackInput feedback)
    {
        feedback.Id = YitIdHelper.NextId();
        var result = await _feedbackRep.InsertAsync(feedback.Adapt<Feedback>());
        await _juFileService.PostFileRelationId(feedback.FileIds, feedback.Id.Value, FileRelationEnum.Feedback);
        return result;
    }

    [DisplayName("反馈列表"), AllowAnonymous]
    public async Task<object> GetFeedbacks([FromRoute] int page, [FromQuery] int size)
    {
        return await _feedbackRep.AsQueryable().CrossQuery(typeof(SysFile), DbConst.AdminDBConfigId)
        .Includes(m => m.Files)
        .Includes(m => m.CreateUser)
        .OrderByDescending(m => m.Id)
        .Select(m => new
        {
            m.Id,
            m.IsAnonymous,
            m.Content,
            m.CreateUserId,
            m.CreateUser.NickName,
            m.CreateUser.Avatar,
            Files = m.Files.Select(f => new { f.Url, f.Id, f.SizeKb }).ToList()
        }).ToPagedListAsync(page, size);
    }
}
