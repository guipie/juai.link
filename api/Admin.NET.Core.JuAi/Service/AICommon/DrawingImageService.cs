
using Newtonsoft.Json.Linq;

namespace Admin.NET.Core.JuAI.Service;
[ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 2)]
public class DrawingImageService : AppBaseService
{
    private readonly SqlSugarRepository<ChatGPTImage> _chagptImageRep;
    private readonly UserManager _userManager;
    public DrawingImageService(SqlSugarRepository<ChatGPTImage> chatgptImageRep, UserManager userManager)
    {
        _chagptImageRep = chatgptImageRep;
        _userManager = userManager;
    }
    [ApiDescriptionSettings(Description = "获取推荐绘画"), AllowAnonymous]
    public async Task<IList<ChatgptImageRes>> GetRecommend([FromRoute] DrawingImageType imageType, [FromQuery] DateTime? minDate)
    {
        return await _chagptImageRep.AsQueryable().Where(m => m.ContentStatus == ContentStatusEnum.Recommend && m.DrawingType == imageType).Take(10)
                     .WhereIF(minDate != null, m => m.CreateTime < minDate)
                     .OrderByDescending(m => m.CreateTime)
                     .Select(m => new ChatgptImageRes()
                     {
                         Id = m.Id,
                         Url = SqlFunc.IsNullOrEmpty(m.OssUrl) ? m.SourceUrl : m.OssUrl,
                     }, true).ToListAsync();
    }
    [ApiDescriptionSettings(Description = "获取绘画详情"), AllowAnonymous]
    public async Task<ChatgptImageDetailRes> Get([FromRoute] long id)
    {
        ChatGPTImage detail = await _chagptImageRep.GetByIdAsync(id);
        var result = detail.Adapt<ChatgptImageDetailRes>();
        result.Url = string.IsNullOrWhiteSpace(detail.OssUrl) ? detail.SourceUrl : detail.OssUrl;
        result.DrawingType = detail.DrawingType.GetDescription();
        if (detail.RequestParameters != null)
        {
            try
            {
                result.Txt = detail.RequestParameters["txt"]?.ToString();
                result.ControlImage = detail.RequestParameters["control_image"]?.ToString();
            }
            catch (Exception)
            {

            }
        }
        return result;
    }
    public async Task<IList<ChatgptImageRes>> GetMy(DrawingImageType? imageType, long? minId)
    {
        return await _chagptImageRep.AsQueryable().Where(m => _userManager.UserId == m.CreateUserId).Take(10)
                     .WhereIF(imageType > 0, m => m.DrawingType == imageType)
                     .WhereIF(minId > 0, m => m.Id < minId)
                     .OrderByDescending(m => m.Id)
                     .Select(m => new ChatgptImageRes()
                     {
                         Id = m.Id,
                         Url = SqlFunc.IsNullOrEmpty(m.OssUrl) ? m.SourceUrl : m.OssUrl,
                     }, true).ToListAsync();
    }
}
