
namespace Admin.NET.Core.JuAI.Service;

[Route("app/api/[controller]")]
[ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 1)]
public class AppBaseService : IDynamicApiController, IScoped
{
}
