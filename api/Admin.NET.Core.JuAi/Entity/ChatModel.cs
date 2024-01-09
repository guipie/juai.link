namespace Admin.NET.Core.JuAI;
[SugarTable("ChatModel", "AI模型"), IncreTable]
[Tenant("app")]
public class ChatModel : EntityAppBase
{

    [SugarColumn(ColumnDescription = "模型", Length = 200)]
    public string ModelId { get; set; } = string.Empty;

    [SugarColumn(ColumnDescription = "模型", Length = 200)]
    public string Name { get; set; } = string.Empty;

    [SugarColumn(ColumnDescription = "模型短称", Length = 200)]
    public string ShortName { get; set; } = string.Empty;

    [SugarColumn(ColumnDescription = "模型头像", IsNullable = true)]
    public string? AvatarUrl { get; set; }

    [SugarColumn(ColumnDescription = "模型类型")]
    public AIModelEnum ModelType { get; set; } = AIModelEnum.Chat;

    [SugarColumn(ColumnDescription = "模型种类供应商")]
    public string Category { get; set; } = string.Empty;

    [SugarColumn(ColumnDescription = "接口地址", Length = 200)]
    public string? Url { get; set; }

    [SugarColumn(ColumnDescription = "最大token数量")]
    public int MaxToken { get; set; }


    [SugarColumn(ColumnDescription = "模型描述", IsNullable = true, Length = 2000)]
    public string Desc { get; set; } = string.Empty;
}
