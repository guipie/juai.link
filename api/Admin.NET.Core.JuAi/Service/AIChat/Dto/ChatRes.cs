
namespace Admin.NET.Core.JuAI.Service;
public class ChatRes
{
    public ChatRes(long chatdbId, string text, string role, long conversationId)
    {
        this.chatDbId = chatdbId;
        this.text = text;
        this.role = role;
        this.conversationId = conversationId;
    }

    public long chatDbId { get; set; }
    public string text { get; set; }
    public string role { get; set; } 
    public long conversationId { get; set; }
}
