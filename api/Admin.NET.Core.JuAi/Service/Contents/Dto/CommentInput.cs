namespace Admin.NET.Core.JuAI.Service;

/// <summary>
/// 新增评论
/// </summary>
public class CommentInput
{
    /// <summary>
    /// 评论
    /// </summary> 
    [Required(ErrorMessage = "评论不能为空"), MaxLength(200, ErrorMessage = "标题不能大于200字符")]
    public string Content { get; set; } = string.Empty;
    /// <summary>
    /// 评论对象
    /// </summary>
    [Required(ErrorMessage = "评论对象不能为空")]
    public long ContentID { get; set; }

    [Required(ErrorMessage = "评论对象不能为空")]
    public ContentTypeEnum CommentType { get; set; }

    public bool? IsAnonymous { get; set; }

    public long? ReplyId { get; set; }

}