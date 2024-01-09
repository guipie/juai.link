namespace Admin.NET.Core.JuAI.Service;

/// <summary>
/// 输出
/// </summary>
public class ArticleSiderOutput
{
    public ArticleSiderOutput(long id, string title)
    {
        Id = id;
        Title = title;
    }
    public ArticleSiderOutput() { }
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;

}