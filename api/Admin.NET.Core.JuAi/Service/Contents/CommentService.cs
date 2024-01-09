
namespace Admin.NET.Core.JuAI.Service;
[ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 100)]
public class CommentService : AppBaseService
{
    private readonly SqlSugarRepository<Comment> _commentRep;
    private readonly SqlSugarRepository<Article> _articleRep;
    private readonly SqlSugarRepository<ChatGPT> _chatRep;
    private readonly UserManager _userManage;
    public CommentService(SqlSugarRepository<Comment> commentRep,
        SqlSugarRepository<Article> articleRep,
        SqlSugarRepository<ChatGPT> chatRep,
        UserManager userManager)
    {
        _commentRep = commentRep;
        _articleRep = articleRep;
        _chatRep = chatRep;
        _userManage = userManager;
    }
    [DisplayName("评论列表")]
    [AllowAnonymous]
    public async Task<IList<CommentOutput>> Get([FromRoute] ContentTypeEnum type,
                                                [FromRoute] long contentId,
                                                [FromQuery] long? minCommentId,
                                                [FromQuery] int? size)
    {
        return await _commentRep.AsQueryable()
              .Where(m => m.ContentId == contentId && m.CommentType == type && SqlFunc.IsNullOrEmpty(m.ReplyId))
              .WhereIF(minCommentId > 0, m => m.Id < minCommentId)
              .Includes(m => m.CreateUser)
              .OrderByDescending(m => m.Id).Take(size ?? 10)
              .Select(m => new CommentOutput() { CreateUser = m.CreateUser.Adapt<UserOutput>() }, true).ToListAsync();
    }
    [DisplayName("回复评论列表")]
    [AllowAnonymous]
    public async Task<IList<CommentOutput>> GetReply([FromRoute] long replyId, [FromQuery] long? minReCommentId, [FromQuery] int? size)
    {
        return await _commentRep.AsQueryable()
              .Where(m => m.ReplyId == replyId)
              .WhereIF(minReCommentId > 0, m => m.Id < minReCommentId)
              .Includes(m => m.CreateUser)
              .OrderByDescending(m => m.Id).Take(size ?? 10)
              .Select(m => new CommentOutput() { CreateUser = m.CreateUser.Adapt<UserOutput>() }, true).ToListAsync();
    }
    [DisplayName("新增评论"), HttpPost]
    public async Task<CommentOutput> Post([FromBody] CommentInput comment)
    {
        var insertModel = await _commentRep.InsertReturnEntityAsync(comment.Adapt<Comment>());
        if (insertModel.Id > 0)
        {
            switch (comment.CommentType)
            {
                case ContentTypeEnum.Article:
                    _articleRep.UpdateSetColumnsTrue(m => new Article() { CommentCount = m.CommentCount + 1 }, m => m.Id == comment.ContentID);
                    break;
                case ContentTypeEnum.Chat:
                    _chatRep.UpdateSetColumnsTrue(m => new ChatGPT() { CommentCount = m.CommentCount + 1 }, m => m.Id == comment.ContentID);
                    break;
            }
            if (comment.ReplyId > 0)
                _commentRep.UpdateSetColumnsTrue(m => new Comment() { ReplyCnt = m.ReplyCnt + 1 }, m => m.Id == comment.ReplyId);

        }
        return insertModel.Adapt<CommentOutput>();
    }
}
