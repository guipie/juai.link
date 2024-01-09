
using Nest;

namespace Admin.NET.Core.JuAI.Service;
public class MidjourneyService : IScoped
{

    public readonly SqlSugarRepository<MidjourneyTask> _midTaskRepository;
    public readonly SqlSugarRepository<MidjourneyNotify> _midNotifyRepository;
    public readonly UserManager _userManager;
    public MidjourneyService(SqlSugarRepository<MidjourneyTask> midjourneyRepository, SqlSugarRepository<MidjourneyNotify> midNotifyRepository, UserManager userManager)
    {
        _midTaskRepository = midjourneyRepository;
        _midNotifyRepository = midNotifyRepository;
        _userManager = userManager;
    }
    public async Task<MidjourneyTaskRes> GetShareImage(string id)
    {
        var result = await _midNotifyRepository.GetByIdAsync(id);
        return result.Adapt<MidjourneyTaskRes>();
    }
    public async Task<bool> UpdateOrInsertByTaskRes(MidjourneyTaskRes res, string qnUrl = "")
    {
        var midNotice = _midNotifyRepository.GetById(res.Id);
        if (midNotice == null || midNotice.Id == null)
        {
            var addMid = res.Adapt<MidjourneyNotify>();
            addMid.OssUrl = qnUrl;
            return await _midNotifyRepository.InsertAsync(addMid);
        }
        else
        {
            if (!midNotice.OssUrl.IsNullOrWhiteSpace()) return true;
            var updateMidNotice = res.Adapt<MidjourneyNotify>();
            return await _midNotifyRepository.UpdateAsync(updateMidNotice);
        }
    }
    public async Task<bool> UpdateOssById(string qnUrl, string id)
    {
        return await _midNotifyRepository.UpdateAsync(m => new MidjourneyNotify() { OssUrl = qnUrl }, w => w.Id == id);
    }
    public async Task<string[]> GetMyTaskIds()
    {
        return await _midTaskRepository.AsQueryable().Where(m => m.CreateUserId == _userManager.UserId).Select(m => m.Id).ToArrayAsync();
    }
    public async Task<bool> DelTaskById(string taskId)
    {
        await _midTaskRepository.DeleteByIdAsync(taskId);
        return await _midNotifyRepository.DeleteByIdAsync(taskId);
    }
    public async Task<bool> PostTask(string taskId, string prompt, string promptEn, string? relationTaskId = null)
    {
        return await _midTaskRepository.InsertAsync(new MidjourneyTask()
        {
            Id = taskId,
            RelationTaskId = relationTaskId,
            Prompt = prompt,
            PromptEn = promptEn
        });
    }
    public async Task<List<MidjourneyNotify>> GetNewest(long? submitTime)
    {

        var result = await _midNotifyRepository.AsQueryable()
                                            .Where(m => !SqlFunc.IsNullOrEmpty(m.ImageUrl))
                                            .WhereIF(!submitTime.IsNullOrEmpty(), m => m.SubmitTime < submitTime)
                                            .OrderByDescending(m => m.SubmitTime)
                                            .Take(15).ToListAsync();
        return result;
    }
    public async Task<List<MidjourneyNotify>> GetRecommend(long? submitTime)
    {

        var result = await _midNotifyRepository.AsQueryable()
                                             .Where(m => (m.ContentStatus == ContentStatusEnum.Top || m.ContentStatus == ContentStatusEnum.Recommend) && !SqlFunc.IsNullOrEmpty(m.ImageUrl))
                                             .WhereIF(!submitTime.IsNullOrEmpty(), m => m.SubmitTime < submitTime)
                                             .OrderByDescending(m => m.ContentStatus).OrderByDescending(m => m.SubmitTime)
                                             .Take(15)
                                             .ToListAsync();
        if (result == null || result.Count == 0) return await GetNewest(submitTime);
        return result;
    }
    public async Task<List<MidjourneyTaskRes>> GetMyNotifyTasks(DateTime? createTime)
    {

        var result = await _midTaskRepository.AsQueryable().LeftJoin<MidjourneyNotify>((mid, notice) => mid.Id == notice.Id)
                                             .Where((mid, notice) => mid.CreateUserId == _userManager.UserId)
                                             .WhereIF(createTime != null && createTime.HasValue, (mid, notice) => mid.CreateTime <= createTime)
                                             .Take(10).OrderByDescending((mid, notice) => mid.CreateTime)
                                             .Select((mid, notice) => new MidjourneyTaskRes() { createTime = mid.CreateTime }, true)
                                             .ToListAsync();
        return result;
    }
    public async Task<bool> PostNotifyInfo(MidjourneyTaskRes res)
    {
        var insertUpdateModel = res.Adapt<MidjourneyNotify>();
        var result = await _midNotifyRepository.AsSugarClient().Storageable(insertUpdateModel).ExecuteCommandAsync();
        return result > 0;
    }

    /// <summary>
    /// 获取未同步oss的mid图片
    /// </summary>
    /// <returns></returns>
    public async Task<IList<MidjourneyNotify>> GetNoOssNoticeTask()
    {
        return await _midNotifyRepository.GetListAsync(m => SqlFunc.IsNullOrEmpty(m.OssUrl) && !SqlFunc.IsNullOrEmpty(m.ImageUrl));
    }

    public async Task<MidjourneyTask> GetTaskByTaskId(string taskId)
    {
        return await _midTaskRepository.GetFirstAsync(m => m.Id == taskId);
    }
    public async Task<MidjourneyNotify> GetNoticeByTaskId(string taskId)
    {
        return await _midNotifyRepository.GetFirstAsync(m => m.Id == taskId);
    }
    public bool IsChanged(int index, string action, string originTaskId)
    {
        return _midTaskRepository.IsAny(m => m.Prompt == originTaskId && m.PromptEn == action + index);
    }

}
