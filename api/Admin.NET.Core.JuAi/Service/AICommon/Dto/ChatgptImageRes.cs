using Admin.NET.Core.JuAI.Entity;

namespace Admin.NET.Core.JuAI.Service;
public class ChatgptImageRes
{
    public virtual long Id { get; set; }
    public string Prompt { get; set; } = string.Empty;

    public string PromptEn { get; set; } = string.Empty;

    public string Url { get; set; }
}

public class ChatgptImageDetailRes : ChatgptImageRes
{
    public string Txt { get; set; }
    public string DrawingType { get; set; }

    public string ControlImage { get; set; }
}