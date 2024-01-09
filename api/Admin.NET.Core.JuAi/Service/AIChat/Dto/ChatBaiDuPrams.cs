

namespace Admin.NET.Core.JuAI.Service.AI.Dto;
public partial class ChatBaiDuAccessReq
{
    public string refresh_token { get; set; }

    public int expires_in { get; set; }

    public string session_key { get; set; }

    public string access_token { get; set; }

    public string scope { get; set; }

    public string session_secret { get; set; }
}

public partial class ChatBaiDuChatDataRes
{
    public ChatBaiDuChatRes data { get; set; }
}

public partial class ChatBaiDuChatRes
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("error_code")]
    public long ErrorCode { get; set; }

    [JsonProperty("error_msg")]
    public string ErrorMsg { get; set; }

    [JsonProperty("object")]
    public string Object { get; set; }

    [JsonProperty("created")]
    public long Created { get; set; }

    [JsonProperty("result")]
    public string Result { get; set; }

    [JsonProperty("is_truncated")]
    public bool IsTruncated { get; set; }

    [JsonProperty("need_clear_history")]
    public bool NeedClearHistory { get; set; }

    [JsonProperty("finish_reason")]
    public string FinishReason { get; set; }

    [JsonProperty("usage")]
    public Usage Usage { get; set; }
}
public partial class Usage
{
    [JsonProperty("prompt_tokens")]
    public long PromptTokens { get; set; }

    [JsonProperty("completion_tokens")]
    public long CompletionTokens { get; set; }

    [JsonProperty("total_tokens")]
    public long TotalTokens { get; set; }
}
