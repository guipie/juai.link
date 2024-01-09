
namespace Admin.NET.Core.JuAI.Service;
/// <summary>
/// 广场问答记录
/// </summary>
public class ChatInfoOutput
{
    public long Id { get; set; }
    public long ConversationId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public DateTime? CreateDate { get; set; }
    public int ViewCount { get; set; }  
     
    public int CommentCount { get; set; } 
     
    public int LikeCount { get; set; } 
}
