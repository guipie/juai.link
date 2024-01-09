
namespace Admin.NET.Core.JuAI;
[SugarTable("TokenPayRecord", "token充值记录")]
[Tenant("app")]
public class TokenPayRecord : EntityAppTenant
{
    public string OrderNo { get; set; } = string.Empty;
    public double BePayRmb { get; set; }
    public long BePayNum { get; set; }

    public DateTime BePayDate { get; set; }

    [SugarColumn(IsNullable = true)]
    public double? PayedRmb { get; set; }

    [SugarColumn(IsNullable = true)]
    public long? PayedNum { get; set; }

    [SugarColumn(IsNullable = true)]
    public DateTime? PayedDate { get; set; }
}
