

namespace Admin.NET.Core.JuAI.Service;
[ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 100)]
public class CollectService : AppBaseService
{
    private readonly SqlSugarRepository<Collect> _collectRep;
    private readonly SqlSugarRepository<Comment> _commentRep;
    private readonly SqlSugarRepository<Article> _articleRep;
    private readonly SqlSugarRepository<ChatGPT> _chatRep;
    private readonly UserManager _userManage;
    public CollectService(
        SqlSugarRepository<Comment> commentRep,
        SqlSugarRepository<Article> articleRep,
        SqlSugarRepository<ChatGPT> chatRep,
        SqlSugarRepository<Collect> collectRep,
        UserManager userManager)
    {
        _commentRep = commentRep;
        _articleRep = articleRep;
        _chatRep = chatRep;
        _collectRep = collectRep;
        _userManage = userManager;
    }

    [DisplayName("收藏")]
    public async Task<bool> Post([FromRoute] ContentTypeEnum type, [FromRoute] long pId)
    {
        if (_collectRep.IsAny(m => m.CollectType == type && m.CreateUserId == _userManage.UserId && m.ContentId == pId))
        {
            await _collectRep.DeleteAsync(m => m.CreateUserId == _userManage.UserId && m.ContentId == pId);
            return false;
        }
        else
        {
            switch (type)
            {
                case ContentTypeEnum.Article:
                    _articleRep.UpdateSetColumnsTrue(m => new Article() { LikeCount = m.LikeCount + 1 }, m => m.Id == pId);
                    break;
                case ContentTypeEnum.Chat:
                    _chatRep.UpdateSetColumnsTrue(m => new ChatGPT() { LikeCount = m.LikeCount + 1 }, m => m.Id == pId);
                    break;
                default:
                    return false;
            }
            return await _collectRep.InsertAsync(new Collect() { ContentId = pId, CollectType = type });
        }

    }

}
