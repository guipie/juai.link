namespace Admin.NET.Core.JuAI;

/// <summary>
/// Midjourney表
/// </summary>
[SugarTable("MidjourneyTask", "MidjourneyTask表")]
[Tenant("app")]
public class MidjourneyTask : EntityBaseIgnoreID
{

    [SugarColumn(ColumnDescription = "Id", IsPrimaryKey = true, IsIdentity = false)]
    public string Id { get; set; } = string.Empty;

    [SugarColumn(ColumnDescription = "关联任务ID", IsNullable = true, Length = 20)]
    public string? RelationTaskId { get; set; } = string.Empty;

    [SugarColumn(ColumnDescription = "图片描述", Length = int.MaxValue)]
    public string Prompt { get; set; } = string.Empty;


    [SugarColumn(ColumnDescription = "翻译后的英文描述", Length = int.MaxValue)]
    public string PromptEn { get; set; } = string.Empty;
}
