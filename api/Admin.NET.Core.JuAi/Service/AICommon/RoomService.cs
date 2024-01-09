
using Admin.NET.Core.JuAI.Service.AICommon.Dto;
using System.Collections.Generic;
using System.Linq;

namespace Admin.NET.Core.JuAI.Service;

public class RoomService : AppBaseService
{

    private readonly SqlSugarRepository<ChatPrompt> _promptRep;
    private readonly SqlSugarRepository<ChatPromptExample> _chatPromptExample;
    private readonly SqlSugarRepository<ChatRoom> _roomRep;
    private readonly UserManager _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JuFileService _juFileService;
    private readonly SqlSugarRepository<ChatGPT> _chatgptRep;
    private readonly SqlSugarRepository<ChatModel> _chatModelRep;
    public RoomService(SqlSugarRepository<ChatPrompt> promptRep, UserManager userManager, SqlSugarRepository<ChatPromptExample> chatPromptExample, IHttpContextAccessor httpContextAccessor, JuFileService juFileService, SqlSugarRepository<ChatRoom> roomRep, SqlSugarRepository<ChatGPT> chatgptRep, SqlSugarRepository<ChatModel> chatModelRep)
    {
        _promptRep = promptRep;
        _userManager = userManager;
        _chatPromptExample = chatPromptExample;
        _httpContextAccessor = httpContextAccessor;
        _juFileService = juFileService;
        _roomRep = roomRep;
        _chatgptRep = chatgptRep;
        _chatModelRep = chatModelRep;
    }
    [ApiDescriptionSettings(Description = "获取我的房间")]
    public async Task<IList<ChatRoom>> GetMy()
    {
        var reslut = await _roomRep.GetListAsync(m => m.CreateUserId == _userManager.UserId);
        return reslut.OrderByDescending(m => m.CreateTime).ToList();
    }
    [ApiDescriptionSettings(Description = "删除我的房间")]
    public async Task<bool> DeleteMy([FromRoute] long id)
    {
        return await _roomRep.DeleteAsync(m => m.CreateUserId == _userManager.UserId && m.Id == id);
    }
    [ApiDescriptionSettings(Description = "我的房间创建")]
    public async Task<bool> Post([FromRoute] string ids)
    {
        var arr = ids.Split(",").Select(m => long.Parse(m)).ToList();
        //var existIds = await _roomRep.GetListAsync(m => m.CreateUserId == _userManager.UserId && arr.Contains(m.Id));
        var prompts = await _promptRep.GetListAsync(m => arr.Contains(m.Id));
        var rooms = prompts.Adapt<List<ChatRoom>>();
        foreach (var item in rooms)
        {
            item.PromptId = item.Id;
            item.Id = 0;
            item.LastMessage = item.InitMessage ?? item.Prompt;
        }
        return await _roomRep.InsertRangeAsync(rooms);
    }

    [ApiDescriptionSettings(Description = "我的房间修改")]
    public async Task<bool> Put([FromBody] ChatRoom room)
    {
        return await _roomRep.UpdateAsync(room);
    }

    [ApiDescriptionSettings(Description = "我的房间消息")]
    public async Task<bool> GetMessages([FromRoute] long roomId)
    {
        return await _chatgptRep.DeleteAsync(m => m.CreateUserId == _userManager.UserId && m.ConversationId == roomId);
    }

    [HttpGet, ApiDescriptionSettings(Description = "发送房间消息")]
    public async Task<ChaRoomReq> SendBefore([FromRoute] long roomId, [FromQuery] long? chatDbId)
    {
        var roomInfo = await _roomRep.GetByIdAsync(roomId);
        if (roomInfo == null || roomInfo.Model.IsNullOrEmpty() || roomInfo.Vendor.IsNullOrEmpty())
            throw Oops.Bah("该房间不存在，请新增新的");
        var modelInfo = await _chatModelRep.GetFirstAsync(m => m.ModelId == roomInfo.Vendor + ":" + roomInfo.Model);
        if (modelInfo == null || modelInfo.Url.IsNullOrEmpty())
            throw Oops.Bah("该会话模型已失效，请新增新的");
        ChaRoomReq req = new()
        {
            Model = roomInfo.Model,
            Options = new ChatGptReqOptions()
            {
                MaxContext = roomInfo.MaxContext,
                ChatDbId = chatDbId,
                ConversationId = roomInfo.Id,
                RolePrompt = roomInfo.Prompt,
                UseContext = true
            },
            Url = modelInfo.Url
        };
        return req;
    }
}
