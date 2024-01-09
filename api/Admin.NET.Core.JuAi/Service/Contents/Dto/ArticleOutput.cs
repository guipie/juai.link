namespace Admin.NET.Core.JuAI.Service;

/// <summary>
/// 输出
/// </summary>
public class ArticleAppOutput
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    //public string Html { get; set; } = string.Empty;
    public string? Text { get; set; }

    public string? Cover { get; set; }
    public long? SpecialId { get; set; }
    public string? SpecialName { get; set; }

    public string? CreateTime { get; set; }
    public int Status { get; set; }
    public UserOutput? CreateUser { get; set; }

    public int ViewCount { get; set; } = 1;

    public int CommentCount { get; set; }

    public int LikeCount { get; set; }
}
public class ArticleOutputHtml : ArticleAppOutput
{
    public string Html { get; set; } = "";
}
public class ArticleAppOutputMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<Article, ArticleAppOutput>()
            .Map(t => t.Text, o => o.Text.Length > 200 ? o.Text.Substring(0, 200) : o.Text)
            .Map(member: t => t.CreateTime, s => s.CreateTime.Value.ToString("yyyy-MM-dd"));
    }
}
public class ArticleOutputHtmlMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<Article, ArticleOutputHtml>()
            .Map(t => t.Text, o => o.Text.Length > 200 ? o.Text.Substring(0, 200) : o.Text)
            .Map(member: t => t.CreateTime, s => s.CreateTime.Value.ToString("yyyy-MM-dd"));
    }
}