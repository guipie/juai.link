
namespace Admin.NET.Core.JuAI;

/// <summary>
/// 所有内容的收藏表
/// </summary>
[SugarTable("Collect", "所有内容的收藏表"), IncreTable]
[Tenant("app")]
public class Collect : EntityAppBaseIgnoreUpdate
{

    [SugarColumn(ColumnDescription = "收藏内容类别")]
    public ContentTypeEnum CollectType { get; set; }


    [SugarColumn(ColumnDescription = "收藏内容ID")]
    public long ContentId { get; set; }

}
