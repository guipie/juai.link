using System.ComponentModel.DataAnnotations;

namespace Admin.NET.Application.JuAI;

    /// <summary>
    /// Mid绘画基础输入参数
    /// </summary>
    public class MidjourneyNotifyBaseInput
    {
        /// <summary>
        /// 类型
        /// </summary>
        public virtual string Action { get; set; }
        
        /// <summary>
        /// 中文
        /// </summary>
        public virtual string Prompt { get; set; }
        
        /// <summary>
        /// 英文
        /// </summary>
        public virtual string? PromptEn { get; set; }
        
        /// <summary>
        /// description
        /// </summary>
        public virtual string? Description { get; set; }
        
        /// <summary>
        /// state
        /// </summary>
        public virtual string? State { get; set; }
        
        /// <summary>
        /// 提交时间
        /// </summary>
        public virtual long? SubmitTime { get; set; }
        
        /// <summary>
        /// start_time
        /// </summary>
        public virtual long? StartTime { get; set; }
        
        /// <summary>
        /// finish_time
        /// </summary>
        public virtual long? FinishTime { get; set; }
        
        /// <summary>
        /// image_url
        /// </summary>
        public virtual string? ImageUrl { get; set; }
        
        /// <summary>
        /// 状态
        /// </summary>
        public virtual string? Status { get; set; }
        
        /// <summary>
        /// progress
        /// </summary>
        public virtual string? Progress { get; set; }
        
        /// <summary>
        /// fail_reason
        /// </summary>
        public virtual string? FailReason { get; set; }
        
        /// <summary>
        /// properties
        /// </summary>
        public virtual string? Properties { get; set; }
        
        /// <summary>
        /// 云地址
        /// </summary>
        public virtual string? OssUrl { get; set; }
        
    }

    /// <summary>
    /// Mid绘画分页查询输入参数
    /// </summary>
    public class MidjourneyNotifyInput : BasePageInput
    {
        /// <summary>
        /// 类型
        /// </summary>
        public string Action { get; set; }
        
        /// <summary>
        /// 中文
        /// </summary>
        public string Prompt { get; set; }
        
        /// <summary>
        /// 英文
        /// </summary>
        public string? PromptEn { get; set; }
        
        /// <summary>
        /// 提交时间
        /// </summary>
        public long? SubmitTime { get; set; }
        
    }

    /// <summary>
    /// Mid绘画增加输入参数
    /// </summary>
    public class AddMidjourneyNotifyInput : MidjourneyNotifyBaseInput
    {
    }

    /// <summary>
    /// Mid绘画删除输入参数
    /// </summary>
    public class DeleteMidjourneyNotifyInput : BaseIdInput
    {
    }

    /// <summary>
    /// Mid绘画更新输入参数
    /// </summary>
    public class UpdateMidjourneyNotifyInput : MidjourneyNotifyBaseInput
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required(ErrorMessage = "Id不能为空")]
        public string Id { get; set; }
        
    }

    /// <summary>
    /// Mid绘画主键查询输入参数
    /// </summary>
    public class QueryByIdMidjourneyNotifyInput : DeleteMidjourneyNotifyInput
    {

    }
