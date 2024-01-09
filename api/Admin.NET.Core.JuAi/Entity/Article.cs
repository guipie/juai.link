
namespace Admin.NET.Core.JuAI;
[SugarTable("Article", "文章")]
[Tenant("app")]
public class Article : EntityAppBase
{
    [SugarColumn(Length = 50)]
    public string Title { get; set; } = string.Empty;

    [SugarColumn(Length = int.MaxValue)]
    public string Html { get; set; } = string.Empty;

    [SugarColumn(Length = int.MaxValue)]
    public string Text { get; set; } = string.Empty;

    [SugarColumn(Length = 200)]
    public string? Cover { get; set; }


    [SugarColumn(ColumnDescription = "浏览次数")]
    public int ViewCount { get; set; } = 1;

    [SugarColumn(ColumnDescription = "评论次数")]
    public int CommentCount { get; set; } = 0;

    [SugarColumn(ColumnDescription = "喜欢收藏次数")]
    public int LikeCount { get; set; } = 0;

    [SugarColumn(ColumnDescription = "文章状态")]
    public ContentStatusEnum Status { get; set; }

    [SugarColumn(ColumnDescription = "专栏ID", IsNullable = true)]
    public long SpecialId { get; set; }

    [SugarColumn(ColumnDescription = "专栏名称", IsNullable = true, Length = 10)]
    public string? SpecialName { get; set; }


    [Navigate(typeof(SysFileRelation), nameof(SysFileRelation.RelationId), nameof(SysFileRelation.FileId))]//注意顺序
    public List<SysFile>? Files { get; set; }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Navigate(NavigateType.OneToOne, nameof(CreateUserId))]
    public new AppUser? CreateUser { get; set; }
}
