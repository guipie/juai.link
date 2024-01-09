using System.ComponentModel.DataAnnotations;

namespace Admin.NET.Application.JuAI;

    /// <summary>
    /// AI会话基础输入参数
    /// </summary>
    public class ChatGPTBaseInput
    {
        /// <summary>
        /// 问题
        /// </summary>
        public virtual string Question { get; set; }
        
        /// <summary>
        /// 答案
        /// </summary>
        public virtual string Answer { get; set; }
        
        /// <summary>
        /// 问(token数)
        /// </summary>
        public virtual int ReqNum { get; set; }
        
        /// <summary>
        /// 答(token数)
        /// </summary>
        public virtual int ResNum { get; set; }
        
        /// <summary>
        /// 模型
        /// </summary>
        public virtual string Model { get; set; }
        
        /// <summary>
        /// 会话ID
        /// </summary>
        public virtual long ConversationId { get; set; }
        
        /// <summary>
        /// IsRepeat
        /// </summary>
        public virtual bool Isrepeat { get; set; }
        
    }

    /// <summary>
    /// AI会话分页查询输入参数
    /// </summary>
    public class ChatGPTInput : BasePageInput
    {
        /// <summary>
        /// 问题
        /// </summary>
        public string Question { get; set; }
        
        /// <summary>
        /// 答案
        /// </summary>
        public string Answer { get; set; }
        
        /// <summary>
        /// 问(token数)
        /// </summary>
        public int ReqNum { get; set; }
        
        /// <summary>
        /// 答(token数)
        /// </summary>
        public int ResNum { get; set; }
        
        /// <summary>
        /// 模型
        /// </summary>
        public string Model { get; set; }
        
    }

    /// <summary>
    /// AI会话增加输入参数
    /// </summary>
    public class AddChatGPTInput : ChatGPTBaseInput
    {
    }

    /// <summary>
    /// AI会话删除输入参数
    /// </summary>
    public class DeleteChatGPTInput : BaseIdInput
    {
    }

    /// <summary>
    /// AI会话更新输入参数
    /// </summary>
    public class UpdateChatGPTInput : ChatGPTBaseInput
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required(ErrorMessage = "Id不能为空")]
        public long Id { get; set; }
        
    }

    /// <summary>
    /// AI会话主键查询输入参数
    /// </summary>
    public class QueryByIdChatGPTInput : DeleteChatGPTInput
    {

    }
