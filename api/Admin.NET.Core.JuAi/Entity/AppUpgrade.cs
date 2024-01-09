
namespace Admin.NET.Core.JuAI;
[SugarTable("AppUpgrade", "用户表")]
[Tenant("app")]
public class AppUpgrade : EntityAppBase
{
    [SugarColumn(Length = 20)]
    public string Version { get; set; } = string.Empty;

    [SugarColumn(IsNullable = true, Length = 2000)]
    public string? Desc { get; set; }

    [SugarColumn(IsNullable = true)]
    public bool? IsForceUpdate { get; set; }

    [SugarColumn(IsNullable = true)]
    public bool? IsClearCache { get; set; }
}
