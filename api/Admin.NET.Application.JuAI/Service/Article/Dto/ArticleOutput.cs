namespace Admin.NET.Application.JuAI;

/// <summary>
/// 聚AI内容输出参数
/// </summary>
public class ArticleOutput
{
    /// <summary>
    /// ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    public string Html { get; set; }

    /// <summary>
    /// 文章描述
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// 封面
    /// </summary>
    public string? Cover { get; set; }
    public SysFile CoverAttachment { get; set; }

    /// <summary>
    /// 浏览次数
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// 评论次数
    /// </summary>
    public int CommentCount { get; set; }

    /// <summary>
    /// 喜欢收藏次数
    /// </summary>
    public int LikeCount { get; set; }

    /// <summary>
    /// 文章状态
    /// </summary>
    public ContentStatusEnum Status { get; set; }

    /// <summary>
    /// 专栏
    /// </summary>
    public long? SpecialId { get; set; }


    /// <summary>
    /// 专栏名称
    /// </summary>
    public string? SpecialName { get; set; }

}


