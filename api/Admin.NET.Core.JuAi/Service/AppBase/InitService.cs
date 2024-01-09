namespace Admin.NET.Core.JuAI.Service;

/// <summary>
/// 数据初始化
/// </summary>
[ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 1)]
public class InitService : AppBaseService
{

    private readonly SqlSugarRepository<ChatModel> _modelRep;
    private readonly JuFileService _juFileService;
    public InitService(SqlSugarRepository<ChatModel> modelRep, JuFileService juFileService)
    {
        _modelRep = modelRep;
        _juFileService = juFileService;
    }
    public async Task<bool> PostModels([FromBody] List<InitModels> initModels)
    {
        var ids = await _modelRep.GetListAsync();
        await _modelRep.DeleteByIdsAsync(ids.Select(m => (dynamic)m.Id).ToArray());
        List<ChatModel> chatModels = new();
        foreach (InitModels model in initModels)
        {
            ChatModel chatModel = model.Adapt<ChatModel>();
            if (model.IsImage) { chatModel.ModelType = AIModelEnum.Image; }
            if (model.IsChat) chatModel.ModelType = AIModelEnum.Chat;
            if (!model.AvatarUrl.IsNullOrEmpty())
            {
                var file = await _juFileService.ImageAsync("init", new SysFile() { Url = model.AvatarUrl, FileName = Path.GetFileName(model.AvatarUrl), FilePath = "init/" + Path.GetFileName(model.AvatarUrl) });
                if (file == null || file.Url.IsNullOrEmpty()) continue;
                chatModel.AvatarUrl = file.Url!;
            }
            else
                chatModel.AvatarUrl = "";
            chatModels.Add(chatModel);
            await _modelRep.InsertAsync(chatModel);
        }
        return true;
    }
}
