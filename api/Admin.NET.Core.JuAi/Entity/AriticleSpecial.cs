
namespace Admin.NET.Core.JuAI;
[SugarTable("Special", "专栏")]
[Tenant("app")]
public class Special : EntityAppBase
{
    [SugarColumn(Length = 10)]
    public string Title { get; set; } = string.Empty;


    [SugarColumn(Length = 200)]
    public string Text { get; set; } = string.Empty;

    [SugarColumn(Length = 200)]
    public string? Cover { get; set; }


    [SugarColumn(ColumnDescription = "专栏状态")]
    public ContentStatusEnum Status { get; set; }


    //[SugarColumn(ColumnDescription = "是否付费")]
    //public bool? IsPay { get; set; }


    //[SugarColumn(ColumnDescription = "是否仅自己发文")]
    //public bool? IsPubMyself { get; set; }
}
