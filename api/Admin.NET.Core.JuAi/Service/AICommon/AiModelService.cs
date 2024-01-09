

namespace Admin.NET.Core.JuAI;

[ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 1)]
public class AiModelService : AppBaseService
{

    private readonly SqlSugarRepository<ChatModel> _modelRep;
    public AiModelService(SqlSugarRepository<ChatModel> modelRep)
    {
        _modelRep = modelRep;
    }
    [AllowAnonymous]
    public async Task<IList<ChatModel>> GetModels()
    {
        return await _modelRep.GetListAsync();
    }
}
