
namespace Admin.NET.Core.JuAI.Service;

public partial class MidjourneyTaskRes
{
    [JsonProperty("action")]
    public string Action { get; set; } = string.Empty;

    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("prompt")]
    public string Prompt { get; set; } = string.Empty;

    [JsonProperty("promptEn")]
    public string PromptEn { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("state")]
    public string State { get; set; } = string.Empty;

    [JsonProperty("submitTime")]
    public long? SubmitTime { get; set; }

    [JsonProperty("startTime")]
    public long? StartTime { get; set; }

    [JsonProperty("finishTime")]
    public long? FinishTime { get; set; }

    [JsonProperty("imageUrl")]
    public string? ImageUrl { get; set; }
    public string? OssUrl { get; set; }

    [JsonProperty("status")]
    public string? Status { get; set; }

    [JsonProperty("progress")]
    public string? Progress { get; set; }

    [JsonProperty("failReason")]
    public string? FailReason { get; set; }

    [JsonProperty("properties")]
    public object? Properties { get; set; }

    public bool Successed { get { return this.Status == "SUCCESS"; } }


    public DateTime? createTime { get; set; }
}