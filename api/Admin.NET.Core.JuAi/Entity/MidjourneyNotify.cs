
namespace Admin.NET.Core.JuAI;

/// <summary>
/// Midjourney表
/// </summary>
[SugarTable("MidjourneyNotify", "MidjourneyNotify")]
[Tenant("app")]
public class MidjourneyNotify : EntityBaseIgnoreID, IDeletedFilter
{
    //public MidjourneyNotify(string id, string action, string prompt, string promptEn, string description, string state, string imageUrl, string status)
    //{
    //    this.Id = id;
    //    this.Action = action;
    //    this.Prompt = prompt;
    //    this.PromptEn = promptEn;
    //    this.Description = description;
    //    this.State = state;
    //    this.ImageUrl = imageUrl;
    //    this.Status = status;
    //}

    [SugarColumn(ColumnDescription = "Id", IsPrimaryKey = true, IsIdentity = false)]
    public string Id { get; set; } = string.Empty;

    [SugarColumn(Length = 20)]
    public string Action { get; set; } = string.Empty;


    [SugarColumn(Length = int.MaxValue)]
    public string Prompt { get; set; } = string.Empty;

    [SugarColumn(IsNullable = true, Length = int.MaxValue)]
    public string? PromptEn { get; set; } = string.Empty;

    [SugarColumn(Length = int.MaxValue)]
    public string? Description { get; set; }

    [SugarColumn(Length = 20)]
    public string? State { get; set; } = string.Empty;

    [SugarColumn(IsNullable = true)]
    public long? SubmitTime { get; set; }

    [SugarColumn(IsNullable = true)]
    public long? StartTime { get; set; }

    [SugarColumn(IsNullable = true)]
    public long? FinishTime { get; set; }

    [SugarColumn(IsNullable = true)]
    public string? ImageUrl { get; set; }

    [SugarColumn(Length = 20)]
    public string? Status { get; set; }

    [SugarColumn(IsNullable = true, Length = 20)]
    public string? Progress { get; set; }

    [SugarColumn(IsNullable = true, Length = 500)]
    public string? FailReason { get; set; }

    [SugarColumn(IsNullable = true, Length = 2000)]
    public string? Properties { get; set; }

    [SugarColumn(IsNullable = true, Length = 200)]
    public string? OssUrl { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "内容状态")]
    public ContentStatusEnum? ContentStatus { get; set; } = ContentStatusEnum.Passed;
}
