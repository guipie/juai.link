namespace Admin.NET.Core.JuAI.Service;

/// <summary>
/// 评论
/// </summary>
public class CommentOutput
{
    public long Id { get; set; }
    public string Content { get; set; } = string.Empty;


    public ContentTypeEnum CommentType { get; set; }

    public bool? IsAnonymous { get; set; }

    public long? ReplyId { get; set; }
    public int ReplyCnt { get; set; }

    public int PraiseCnt { get; set; }
    public DateTime? CreateTime { get; set; }
    public UserOutput? CreateUser { get; set; } = new UserOutput() { NickName = "匿名" };
}