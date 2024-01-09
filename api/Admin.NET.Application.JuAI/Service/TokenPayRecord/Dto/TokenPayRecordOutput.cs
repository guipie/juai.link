namespace Admin.NET.Application.JuAI;

    /// <summary>
    /// 充值记录输出参数
    /// </summary>
    public class TokenPayRecordOutput
    {
       /// <summary>
       /// Id
       /// </summary>
       public long Id { get; set; }
    
       /// <summary>
       /// 订单号
       /// </summary>
       public string Orderno { get; set; }
    
       /// <summary>
       /// 预充值金额
       /// </summary>
       public double Bepayrmb { get; set; }
    
       /// <summary>
       /// 预充值数量
       /// </summary>
       public long Bepaynum { get; set; }
    
       /// <summary>
       /// 预充值日期
       /// </summary>
       public DateTime Bepaydate { get; set; }
    
       /// <summary>
       /// 充值金额
       /// </summary>
       public double? PayedRmb { get; set; }
    
       /// <summary>
       /// 充值数量
       /// </summary>
       public long? PayedNum { get; set; }
    
       /// <summary>
       /// 充值日期
       /// </summary>
       public DateTime? PayedDate { get; set; }
    
    }
 

