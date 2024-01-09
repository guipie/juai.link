
namespace Admin.NET.Core.JuAI;

/// <summary>
/// 文章点赞表
/// </summary>
[SugarTable("PraiseArticle", "文章点赞表"), IncreTable]
[Tenant("app")]
public class PraiseArticle : EntityAppBaseIgnoreUpdate
{
    public long ContentId { get; set; }
}
