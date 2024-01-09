
namespace Admin.NET.Core.JuAI;

/// <summary>
/// 用户表
/// </summary>
[SugarTable("ChatRoom", "ChatRoom表")]
[Tenant("app")]
public class ChatRoom : EntityAppBase
{

    [SugarColumn(ColumnDescription = "数字人ID")]
    [Required]
    public long PromptId { get; set; }


    [SugarColumn(ColumnDescription = "标题", Length = 50)]
    [Required]
    public string Title { get; set; } = string.Empty;


    [SugarColumn(ColumnDescription = "提示词", Length = 4000)]
    [Required]
    public string Prompt { get; set; } = string.Empty;


    [SugarColumn(ColumnDescription = "最大上下文引用", DefaultValue = "3")]
    public int MaxContext { get; set; }


    [SugarColumn(ColumnDescription = "最后的消息", Length = 4000, IsNullable = true)]
    public string LastMessage { get; set; } = string.Empty;



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
