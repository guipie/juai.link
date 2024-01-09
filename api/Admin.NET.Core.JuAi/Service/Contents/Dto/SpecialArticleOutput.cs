namespace Admin.NET.Core.JuAI.Service;

/// <summary>
/// 输出
/// </summary>
public class SpecialArticleOutput
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;

    public IList<SysFile>? Files { get; set; }
}