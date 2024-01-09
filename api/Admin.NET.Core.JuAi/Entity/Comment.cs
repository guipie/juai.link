
namespace Admin.NET.Core.JuAI;

/// <summary>
/// 所有内容的评论表
/// </summary>
[SugarTable("Comment", "所有内容的评论表"), IncreTable]
[Tenant("app")]
public class Comment : EntityAppBaseIgnoreUpdate
{
    [SugarColumn(ColumnDescription = "评论内容", Length = 200, IsNullable = false)]
    [Required]
    public string Content { get; set; } = string.Empty;

    [Required, SugarColumn(ColumnDescription = "评论对象的ID", IsNullable = false)]
    public long ContentId { get; set; }

    [SugarColumn(ColumnDescription = "评论类别")]
    public ContentTypeEnum CommentType { get; set; }


    [SugarColumn(ColumnDescription = "是否匿名")]
    public bool? IsAnonymous { get; set; }



    [SugarColumn(ColumnDescription = "回复父级ID")]
    public long? ReplyId { get; set; }

    [SugarColumn(ColumnDescription = "回复数量")]
    public int ReplyCnt { get; set; }

    [SugarColumn(ColumnDescription = "点赞数量")]
    public int PraiseCnt { get; set; }
}
