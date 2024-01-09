
namespace Admin.NET.Core.JuAI;

/// <summary>
/// 评论点赞表
/// </summary>
[SugarTable("PraiseComment", "评论点赞表"), IncreTable]
[Tenant("app")]
public class PraiseComment : EntityAppBaseIgnoreUpdate
{
    public long ContentId { get; set; }
}
