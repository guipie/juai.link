namespace Admin.NET.Core.JuAI.Service;

/// <summary>
/// 新增专栏
/// </summary>
public class SpecialInput
{
    public virtual long? Id { get; set; }

    [Required(ErrorMessage = "标题不能为空"), MaxLength(10, ErrorMessage = "标题不能大于4个字符")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "专栏简介不能为空"), MaxLength(200, ErrorMessage = "专栏简介不能大于200个字符")]
    public string Text { get; set; } = string.Empty;

    public string Cover { get; set; } = "";

    public long CoverFileId { get; set; }

    [AdaptIgnore, JsonIgnore]
    public IFormFile? CoverFile { get; set; }


}

public class SpecialOutput : SpecialInput
{

}