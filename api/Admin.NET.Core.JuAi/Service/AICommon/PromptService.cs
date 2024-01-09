
using Admin.NET.Core.JuAI.Service.AICommon.Dto;
using System.Collections.Generic;
using System.Linq;

namespace Admin.NET.Core.JuAI.Service;

public class PromptService : AppBaseService
{

    private readonly SqlSugarRepository<ChatPrompt> _promptRep;
    private readonly SqlSugarRepository<ChatPromptExample> _chatPromptExample;
    private readonly SqlSugarRepository<ChatRoom> _roomRep;
    private readonly UserManager _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JuFileService _juFileService;
    public PromptService(SqlSugarRepository<ChatPrompt> promptRep, UserManager userManager, SqlSugarRepository<ChatPromptExample> chatPromptExample, IHttpContextAccessor httpContextAccessor, JuFileService juFileService, SqlSugarRepository<ChatRoom> roomRep)
    {
        _promptRep = promptRep;
        _userManager = userManager;
        _chatPromptExample = chatPromptExample;
        _httpContextAccessor = httpContextAccessor;
        _juFileService = juFileService;
        _roomRep = roomRep;
    }
    [AllowAnonymous]
    public async Task<IList<PromptReponse>> Get()
    {
        var reslut = await _promptRep.GetListAsync(m => m.Type == "system");
        return reslut.OrderByDescending(m => m.CreateTime).Adapt<IList<PromptReponse>>();
    }

    /// <summary>
    /// 获取demo示例
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    public async Task<IList<ChatPromptExample>> GetExamples()
    {
        return await _chatPromptExample.GetListAsync();
    }

    [ApiDescriptionSettings(Description = "获取我的自定义数字人")]
    public async Task<IList<PromptReponse>> GetMy()
    {
        var reslut = await _promptRep.GetListAsync(m => m.CreateUserId == _userManager.UserId);
        return reslut.OrderByDescending(m => m.CreateTime).Adapt<IList<PromptReponse>>();
    }

    [ApiDescriptionSettings(Description = "数字人添加修改")]
    public async Task<bool> Post([FromForm] ChatPromptPost post)
    {
        if (post.file != null)
        {
            var ff = await _juFileService.UploadFile(_httpContextAccessor.HttpContext.Request.Form.Files[0], "person/" + DateTime.Now.ToString("yyyyMM"));
            post.avatar = ff.Url;
        }
        return await _promptRep.InsertOrUpdateAsync(post.Adapt<ChatPrompt>());
    }
    [ApiDescriptionSettings(Description = "数字人删除")]
    public async Task<bool> Delete([FromRoute] string ids)
    {
        var arr = ids.Split(",");
        return await _promptRep.DeleteAsync(m => arr.Contains(m.Id.ToString()) && m.CreateUserId == _userManager.UserId);
    }

     
}
