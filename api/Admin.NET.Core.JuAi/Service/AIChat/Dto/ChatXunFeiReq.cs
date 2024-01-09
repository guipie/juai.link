

namespace Admin.NET.Core.JuAI.Service.AI.Dto;
#region 请求体

public class XfJsonRequest
{
    public XfHeader header { get; set; }
    public XfParameter parameter { get; set; }
    public XfPayload payload { get; set; }
}

public class XfHeader
{
    public string app_id { get; set; }
    public string uid { get; set; }
}

public class XfParameter
{
    public XfChat chat { get; set; }
}

public class XfChat
{
    public string domain { get; set; }
    public double temperature { get; set; }
    public int max_tokens { get; set; }
}

public class XfPayload
{
    public XfMessage message { get; set; }
}

public class XfMessage
{
    public List<XfContent> text { get; set; }
}

public class XfContent
{
    public string role { get; set; }
    public string content { get; set; }
}


#endregion

#region 返回体
public partial class ChatXunFeiRes
{
    [JsonProperty("header")]
    public Header Header { get; set; }

    [JsonProperty("payload")]
    public Payload Payload { get; set; }
}

public partial class Header
{
    [JsonProperty("code")]
    public long Code { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("sid")]
    public string Sid { get; set; }

    [JsonProperty("status")]
    public long Status { get; set; }
}

public partial class Payload
{
    [JsonProperty("choices")]
    public Choices Choices { get; set; }

    [JsonProperty("usage")]
    public Usage Usage { get; set; }
}

public partial class Choices
{
    [JsonProperty("status")]
    public long Status { get; set; }

    [JsonProperty("seq")]
    public long Seq { get; set; }

    [JsonProperty("text")]
    public TextElement[] Text { get; set; }
}

public partial class TextElement
{
    [JsonProperty("content")]
    public string Content { get; set; }

    [JsonProperty("role")]
    public string Role { get; set; }

    [JsonProperty("index")]
    public long Index { get; set; }
}

public partial class Usage
{
    [JsonProperty("text")]
    public UsageText Text { get; set; }
}

public partial class UsageText
{
    [JsonProperty("question_tokens")]
    public long QuestionTokens { get; set; }

    [JsonProperty("prompt_tokens")]
    public long PromptTokens { get; set; }

    [JsonProperty("completion_tokens")]
    public long CompletionTokens { get; set; }

    [JsonProperty("total_tokens")]
    public long TotalTokens { get; set; }
}
#endregion