namespace Admin.NET.Application.JuAI;

/// <summary>
/// 专栏输出参数
/// </summary>
public class SpecialDto
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 专栏标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 专栏描述
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// 封面
    /// </summary>
    public string? Cover { get; set; }

    /// <summary>
    /// 专栏状态
    /// </summary>
    public ContentStatusEnum Status { get; set; }

}
