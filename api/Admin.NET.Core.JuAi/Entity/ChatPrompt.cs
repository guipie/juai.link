
namespace Admin.NET.Core.JuAI;

/// <summary>
/// 用户表
/// </summary>
[SugarTable("ChatPrompt", "ChatPrompt表")]
[Tenant("app"), IncreTable]
public class ChatPrompt : EntityAppBase
{
    [SugarColumn(ColumnDescription = "类型", Length = 20, DefaultValue = "system")]
    [Required]
    public string Type { get; set; } = "system";

    [SugarColumn(ColumnDescription = "标题", Length = 50)]
    [Required]
    public string Title { get; set; } = string.Empty;


    [SugarColumn(ColumnDescription = "提示词", Length = 4000)]
    [Required]
    public string Prompt { get; set; } = string.Empty;


    [SugarColumn(ColumnDescription = "最大上下文引用", DefaultValue = "3")]
    public int MaxContext { get; set; }


    [SugarColumn(ColumnDescription = "提示词描述", Length = 4000, IsNullable = true)]
    public string PromptExtend { get; set; } = string.Empty;



    [SugarColumn(ColumnDescription = "头像", IsNullable = true)]
    public string Avatar { get; set; } = string.Empty;

    [SugarColumn(ColumnDescription = "初始化消息", Length = 2000, IsNullable = true)]
    public string InitMessage { get; set; } = string.Empty;


    [SugarColumn(ColumnDescription = "标签", Length = 200, IsNullable = true)]
    public string Tags { get; set; }


    [SugarColumn(ColumnDescription = "模型")]
    public string Model { get; set; } = string.Empty;

    [SugarColumn(ColumnDescription = "供应商")]
    public string Vendor { get; set; } = string.Empty;

}
