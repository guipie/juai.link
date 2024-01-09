using System.ComponentModel.DataAnnotations;

namespace Admin.NET.Application.JuAI;

    /// <summary>
    /// 聚AI内容基础输入参数
    /// </summary>
    public class ArticleBaseInput
    {
        /// <summary>
        /// 标题
        /// </summary>
        public virtual string Title { get; set; }
        
        /// <summary>
        /// 内容
        /// </summary>
        public virtual string Html { get; set; }
        
        /// <summary>
        /// 文章描述
        /// </summary>
        public virtual string Text { get; set; }
        
        /// <summary>
        /// 封面
        /// </summary>
        public virtual string? Cover { get; set; }
        
        /// <summary>
        /// 浏览次数
        /// </summary>
        public virtual int ViewCount { get; set; }
        
        /// <summary>
        /// 评论次数
        /// </summary>
        public virtual int CommentCount { get; set; }
        
        /// <summary>
        /// 喜欢收藏次数
        /// </summary>
        public virtual int LikeCount { get; set; }
        
        /// <summary>
        /// 文章状态
        /// </summary>
        public virtual ContentStatusEnum Status { get; set; }
        
        /// <summary>
        /// 专栏
        /// </summary>
        public virtual long? SpecialId { get; set; }
        
        /// <summary>
        /// 专栏名称
        /// </summary>
        public virtual string? SpecialName { get; set; }
        
    }

    /// <summary>
    /// 聚AI内容分页查询输入参数
    /// </summary>
    public class ArticleInput : BasePageInput
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// 内容
        /// </summary>
        public string Html { get; set; }
        
        /// <summary>
        /// 文章描述
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// 文章状态
        /// </summary>
        public ContentStatusEnum Status { get; set; }
        
        /// <summary>
        /// 专栏
        /// </summary>
        public long? SpecialId { get; set; }
        
    }

    /// <summary>
    /// 聚AI内容增加输入参数
    /// </summary>
    public class AddArticleInput : ArticleBaseInput
    {
    }

    /// <summary>
    /// 聚AI内容删除输入参数
    /// </summary>
    public class DeleteArticleInput : BaseIdInput
    {
    }

    /// <summary>
    /// 聚AI内容更新输入参数
    /// </summary>
    public class UpdateArticleInput : ArticleBaseInput
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public long Id { get; set; }
        
    }

    /// <summary>
    /// 聚AI内容主键查询输入参数
    /// </summary>
    public class QueryByIdArticleInput : DeleteArticleInput
    {

    }
