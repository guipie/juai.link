
namespace Admin.NET.Core.JuAI;

/// <summary>
/// 会话点赞表
/// </summary>
[SugarTable("PraiseChat", "会话点赞表"), IncreTable]
[Tenant("app")]
public class PraiseChat : EntityAppBaseIgnoreUpdate
{
    public long ContentId { get; set; }
}
