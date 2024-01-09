
using Newtonsoft.Json.Linq;

namespace Admin.NET.Core.JuAI;

/// <summary>
/// chatgpt图片表
/// </summary>
[SugarTable("drawing_image", "绘画表"), IncreTable]
[Tenant("app")]
public class ChatGPTImage : EntityAppBaseIgnoreUpdate
{

    [SugarColumn(ColumnDescription = "问题", Length = 2000)]
    [Required, MaxLength(2000)]
    public string Prompt { get; set; } = string.Empty;

    [SugarColumn(ColumnDescription = "问题翻译后的", Length = 8000)]
    [MaxLength(8000)]
    public string PromptEn { get; set; } = string.Empty;


    [SugarColumn(ColumnDescription = "生成的图片原地址", Length = int.MaxValue)]
    public string SourceUrl { get; set; } = string.Empty;

    [SugarColumn(ColumnDescription = "生成的图片云地址", Length = 500)]
    public string OssUrl { get; set; } = string.Empty;

    [SugarColumn(ColumnDescription = "消耗token数")]
    public long Num { get; set; }

    [SugarColumn(ColumnDescription = "请求参数", IsNullable = true, IsJson = true, Length = int.MaxValue)]
    public JObject RequestParameters { get; set; }

    [SugarColumn(ColumnDescription = "绘画返回参数", IsNullable = true, IsJson = true, Length = int.MaxValue)]
    public JObject ResponseParameters { get; set; }

    [SugarColumn(ColumnDescription = "是否成功")]
    public bool IsSuccess { get; set; } = false;

    [SugarColumn(ColumnDescription = "模型")]
    public string? Model { get; set; }


    [SugarColumn(ColumnDescription = "绘图类型-文生图，图生图，艺术字等")]
    public DrawingImageType DrawingType { get; set; } = DrawingImageType.text_image;


    [SugarColumn(IsNullable = true, ColumnDescription = "内容状态")]
    public ContentStatusEnum? ContentStatus { get; set; } = ContentStatusEnum.Passed;



    [Navigate(typeof(SysFileRelation), nameof(SysFileRelation.RelationId), nameof(SysFileRelation.FileId))]//注意顺序
    public List<SysFile>? Files { get; set; }
}
