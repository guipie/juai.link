
namespace Admin.NET.Core.JuAI;

/// <summary>
/// 会话表
/// </summary>
[SugarTable("chat_gpt", "chatgpt问答表"), IncreTable]
[Tenant("app")]
public class ChatGPT : EntityAppTenant
{

    [SugarColumn(ColumnDescription = "问题", Length = 20000)]
    [Required, MaxLength(20000)]
    public string Question { get; set; } = string.Empty;


    [SugarColumn(ColumnDescription = "答案", Length = int.MaxValue)]
    [Required, MaxLength(int.MaxValue)]
    public string Answer { get; set; } = string.Empty;


    [SugarColumn(ColumnDescription = "访问token数")]
    [Required]
    public long ReqNum { get; set; } = 0;


    [SugarColumn(ColumnDescription = "访问token数")]
    [Required]
    public long ResNum { get; set; } = 0;

    public string Model { get; set; } = string.Empty;


    [SugarColumn(ColumnDescription = "会话ID")]
    [Required]
    public long ConversationId { get; set; } = 0;

    /// <summary>
    /// 是否重试了
    /// </summary>
    [SugarColumn(ColumnDescription = "重新生成答案", IsNullable = true)]
    public bool IsRepeat { get; set; } = false;

    [SugarColumn(ColumnDescription = "内容状态", IsNullable = true)]
    public ContentStatusEnum Status { get; set; } = ContentStatusEnum.Passed;



    [SugarColumn(ColumnDescription = "浏览次数")]
    public int ViewCount { get; set; } = 1;

    [SugarColumn(ColumnDescription = "评论次数")]
    public int CommentCount { get; set; } = 0;

    [SugarColumn(ColumnDescription = "喜欢收藏次数")]
    public int LikeCount { get; set; } = 0;
}
