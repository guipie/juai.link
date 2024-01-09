using System.ComponentModel.DataAnnotations;

namespace Admin.NET.Application.JuAI;

    /// <summary>
    /// 专栏基础输入参数
    /// </summary>
    public class SpecialBaseInput
    {
        /// <summary>
        /// 专栏标题
        /// </summary>
        public virtual string Title { get; set; }
        
        /// <summary>
        /// 专栏描述
        /// </summary>
        public virtual string Text { get; set; }
        
        /// <summary>
        /// 封面
        /// </summary>
        public virtual string? Cover { get; set; }
        
        /// <summary>
        /// 专栏状态
        /// </summary>
        public virtual ContentStatusEnum Status { get; set; }
        
    }

    /// <summary>
    /// 专栏分页查询输入参数
    /// </summary>
    public class SpecialInput : BasePageInput
    {
        /// <summary>
        /// 专栏标题
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// 专栏描述
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// 专栏状态
        /// </summary>
        public ContentStatusEnum Status { get; set; }
        
    }

    /// <summary>
    /// 专栏增加输入参数
    /// </summary>
    public class AddSpecialInput : SpecialBaseInput
    {
        /// <summary>
        /// 专栏标题
        /// </summary>
        [Required(ErrorMessage = "专栏标题不能为空")]
        public override string Title { get; set; }
        
        /// <summary>
        /// 专栏描述
        /// </summary>
        [Required(ErrorMessage = "专栏描述不能为空")]
        public override string Text { get; set; }
        
        /// <summary>
        /// 封面
        /// </summary>
        [Required(ErrorMessage = "封面不能为空")]
        public override string? Cover { get; set; }
        
    }

    /// <summary>
    /// 专栏删除输入参数
    /// </summary>
    public class DeleteSpecialInput : BaseIdInput
    {
    }

    /// <summary>
    /// 专栏更新输入参数
    /// </summary>
    public class UpdateSpecialInput : SpecialBaseInput
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required(ErrorMessage = "Id不能为空")]
        public long Id { get; set; }
        
    }

    /// <summary>
    /// 专栏主键查询输入参数
    /// </summary>
    public class QueryByIdSpecialInput : DeleteSpecialInput
    {

    }
