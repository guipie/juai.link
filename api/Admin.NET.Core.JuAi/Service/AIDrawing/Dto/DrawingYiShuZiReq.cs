namespace Admin.NET.Core.JuAI.Service.AIDrawing;

public record DrawingYiShuZiReq
{


    [JsonProperty("control_image")]
    public string ControlImage { get; set; }

    [JsonProperty("model")]
    public string Model { get; set; }

    [JsonProperty("prompt")]
    public string Prompt { get; set; }

    [JsonProperty("negative_prompt")]
    public string NegativePrompt { get; set; } = "";

    [JsonProperty("control_image_ratio")]
    public double ControlImageRatio { get; set; } = 0.75;

    [JsonProperty("control_weight")]
    public double ControlWeight { get; set; } = 1.28;

    [JsonProperty("guidance_start")]
    public double GuidanceStart { get; set; } = 0.3;

    [JsonProperty("guidance_end")]
    public double GuidanceEnd { get; set; } = 0.95;

    [JsonProperty("seed")]
    public long Seed { get; set; } = 1350826899;

    [JsonProperty("steps")]
    public long Steps { get; set; } = 30;

    [JsonProperty("cfg_scale")]
    public long CfgScale { get; set; } = 7;

    //[JsonIgnore]
    public bool IsOpt { get; set; }


    public string Size { get; set; }

    public string Txt { get; set; }

    public string[]? Tags { get; set; }

    public DrawingImageType DrawingType { get; set; }

}