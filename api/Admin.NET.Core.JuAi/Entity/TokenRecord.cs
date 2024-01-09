
namespace Admin.NET.Core.JuAI;
[SugarTable("TokenRecord", "token所有累计记录")]
[Tenant("app")]
public class TokenRecord : EntityAppTenant
{
    public TokenAddTypeEnum TokenAddType { get; set; }

    public long AddNum { get; set; }

    [SugarColumn(IsNullable = true)]
    public string? Extend { get; set; }
}
