namespace Admin.NET.Core.JuAI.Service.AIDrawing;

public record DrawingReq : ImageCreateRequest
{
    public long? Id { get; set; }
    public bool IsOpt { get; set; }
    public string[] Tags { get; set; }
    //public string PromptEn { get; set; } = string.Empty;
}