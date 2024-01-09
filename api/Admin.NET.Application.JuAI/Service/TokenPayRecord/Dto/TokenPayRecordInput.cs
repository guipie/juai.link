using System.ComponentModel.DataAnnotations;

namespace Admin.NET.Application.JuAI;

    /// <summary>
    /// 充值记录基础输入参数
    /// </summary>
    public class TokenPayRecordBaseInput
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public virtual string Orderno { get; set; }
        
        /// <summary>
        /// 预充值金额
        /// </summary>
        public virtual double Bepayrmb { get; set; }
        
        /// <summary>
        /// 预充值数量
        /// </summary>
        public virtual long Bepaynum { get; set; }
        
        /// <summary>
        /// 预充值日期
        /// </summary>
        public virtual DateTime Bepaydate { get; set; }
        
        /// <summary>
        /// 充值金额
        /// </summary>
        public virtual double? PayedRmb { get; set; }
        
        /// <summary>
        /// 充值数量
        /// </summary>
        public virtual long? PayedNum { get; set; }
        
        /// <summary>
        /// 充值日期
        /// </summary>
        public virtual DateTime? PayedDate { get; set; }
        
    }

    /// <summary>
    /// 充值记录分页查询输入参数
    /// </summary>
    public class TokenPayRecordInput : BasePageInput
    {
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
         /// 预充值日期范围
         /// </summary>
         public List<DateTime?> BepaydateRange { get; set; } 
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
        
        /// <summary>
         /// 充值日期范围
         /// </summary>
         public List<DateTime?> PayedDateRange { get; set; } 
    }

    /// <summary>
    /// 充值记录增加输入参数
    /// </summary>
    public class AddTokenPayRecordInput : TokenPayRecordBaseInput
    {
    }

    /// <summary>
    /// 充值记录删除输入参数
    /// </summary>
    public class DeleteTokenPayRecordInput : BaseIdInput
    {
    }

    /// <summary>
    /// 充值记录更新输入参数
    /// </summary>
    public class UpdateTokenPayRecordInput : TokenPayRecordBaseInput
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required(ErrorMessage = "Id不能为空")]
        public long Id { get; set; }
        
    }

    /// <summary>
    /// 充值记录主键查询输入参数
    /// </summary>
    public class QueryByIdTokenPayRecordInput : DeleteTokenPayRecordInput
    {

    }
