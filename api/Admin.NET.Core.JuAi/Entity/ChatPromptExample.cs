
namespace Admin.NET.Core.JuAI;

/// <summary>
/// 用户表
/// </summary>
[SugarTable("ChatPromptExample", "ChatPromptExample表")]
[Tenant("app"), IncreTable]
public class ChatPromptExample : EntityAppBase
{

    [SugarColumn(ColumnDescription = "标题", Length = 50)]
    [Required]
    public string Title { get; set; } = string.Empty;


    [SugarColumn(ColumnDescription = "提示词", Length = 4000, IsNullable = true)]
    public string Prompt { get; set; } = string.Empty;

}
