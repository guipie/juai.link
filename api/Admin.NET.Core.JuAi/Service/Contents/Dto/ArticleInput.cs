namespace Admin.NET.Core.JuAI.Service;

/// <summary>
/// 新增文章
/// </summary>
public class ArticleInput
{
    public long? Id { get; set; }
    /// <summary>
    /// 账号
    /// </summary>
    /// <example>admin</example>
    [Required(ErrorMessage = "标题不能为空"), MinLength(4, ErrorMessage = "标题不能少于4个字符")]
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// 文章HTML
    /// </summary>
    [Required(ErrorMessage = "内容不能为空"), MinLength(200, ErrorMessage = "优质内容不能少于200个字符")]
    public string Html { get; set; } = string.Empty;

    /// <summary>
    /// 文章文本
    /// </summary>
    /// <example>123456</example>
    public string? Text { get; set; }

    [AdaptIgnore]
    public long[]? FileIds { get; set; }

    public string? Cover { get; set; }


    public long? SpecialId { get; set; }
    public string? SpecialName { get; set; }

}