namespace Admin.NET.Core.JuAI.Service;

/// <summary>
/// 模型初始化
/// </summary>
public partial class InitModels
{
    [JsonProperty("id")]
    public string ModelId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("short_name")]
    public string ShortName { get; set; }

    [JsonProperty("description")]
    public string Desc { get; set; }

    [JsonProperty("avatar_url")]
    public string AvatarUrl { get; set; }

    [JsonProperty("category")]
    public string Category { get; set; }

    [JsonProperty("is_image")]
    public bool IsImage { get; set; }

    [JsonProperty("disabled")]
    public bool Disabled { get; set; }

    [JsonProperty("is_chat")]
    public bool IsChat { get; set; }
}