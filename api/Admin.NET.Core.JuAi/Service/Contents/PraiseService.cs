namespace Admin.NET.Core.JuAI.Service;
[ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 100)]
public class PraiseService : AppBaseService
{
    private readonly SqlSugarRepository<PraiseArticle> _praiseArticleRep;
    private readonly SqlSugarRepository<PraiseChat> _praiseChatRep;
    private readonly SqlSugarRepository<PraiseComment> _praiseCommentRep;
    private readonly SqlSugarRepository<Comment> _commentRep;
    private readonly SqlSugarRepository<Article> _articleRep;
    private readonly SqlSugarRepository<ChatGPT> _chatRep;
    private readonly UserManager _userManage;
    public PraiseService(
        SqlSugarRepository<Comment> commentRep,
        SqlSugarRepository<Article> articleRep,
        SqlSugarRepository<ChatGPT> chatRep,
        SqlSugarRepository<PraiseArticle> praiseArticleRep,
        SqlSugarRepository<PraiseChat> praiseChatRep,
        SqlSugarRepository<PraiseComment> praiseCommentRep,
        UserManager userManager)
    {
        _commentRep = commentRep;
        _articleRep = articleRep;
        _chatRep = chatRep;
        _praiseArticleRep = praiseArticleRep;
        _praiseChatRep = praiseChatRep;
        _praiseCommentRep = praiseCommentRep;
        _userManage = userManager;
    }

    [DisplayName("点赞")]
    public async Task<bool> Post([FromRoute] ContentTypeEnum type, [FromRoute] long pId)
    {
        switch (type)
        {
            case ContentTypeEnum.Article:
                if (_praiseArticleRep.IsAny(m => m.CreateUserId == _userManage.UserId && m.ContentId == pId))
                {
                    _articleRep.UpdateSetColumnsTrue(m => new Article() { LikeCount = m.LikeCount - 1 }, m => m.Id == pId);
                    await _praiseArticleRep.DeleteAsync(m => m.CreateUserId == _userManage.UserId && m.ContentId == pId);
                    return false;
                }
                else
                {
                    _articleRep.UpdateSetColumnsTrue(m => new Article() { LikeCount = m.LikeCount + 1 }, m => m.Id == pId);
                    return await _praiseArticleRep.InsertAsync(new PraiseArticle() { ContentId = pId });
                }
            case ContentTypeEnum.Chat:
                if (_praiseChatRep.IsAny(m => m.CreateUserId == _userManage.UserId && m.ContentId == pId))
                {
                    _chatRep.UpdateSetColumnsTrue(m => new ChatGPT() { LikeCount = m.LikeCount - 1 }, m => m.Id == pId);
                    await _praiseChatRep.DeleteAsync(m => m.CreateUserId == _userManage.UserId && m.ContentId == pId);
                    return false;
                }
                else
                {
                    _chatRep.UpdateSetColumnsTrue(m => new ChatGPT() { LikeCount = m.LikeCount + 1 }, m => m.Id == pId);
                    return await _praiseChatRep.InsertAsync(new PraiseChat() { ContentId = pId });
                }
            case ContentTypeEnum.Comment:
                if (_praiseCommentRep.IsAny(m => m.CreateUserId == _userManage.UserId && m.ContentId == pId))
                {
                    _commentRep.UpdateSetColumnsTrue(m => new Comment() { PraiseCnt = m.PraiseCnt - 1 }, m => m.Id == pId);
                    await _praiseCommentRep.DeleteAsync(m => m.CreateUserId == _userManage.UserId && m.ContentId == pId);
                    return false;
                }
                else
                {
                    _commentRep.UpdateSetColumnsTrue(m => new Comment() { PraiseCnt = m.PraiseCnt + 1 }, m => m.Id == pId);
                    return await _praiseCommentRep.InsertAsync(new PraiseComment() { ContentId = pId });
                }
            default:
                return false;
        }
    }

}
