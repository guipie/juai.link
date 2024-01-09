﻿// MIT 许可证
//
// 版权 © 2020-present 百小僧, 百签科技（广东）有限公司
//
// 特此免费授予任何获得本软件副本和相关文档文件（下称“软件”）的人不受限制地处置该软件的权利，
// 包括不受限制地使用、复制、修改、合并、发布、分发、转授许可和/或出售该软件副本，
// 以及再授权被配发了本软件的人如上的权利，须在下列条件下：
//
// 上述版权声明和本许可声明应包含在该软件的所有副本或实质成分中。
//
// 本软件是“如此”提供的，没有任何形式的明示或暗示的保证，包括但不限于对适销性、特定用途的适用性和不侵权的保证。
// 在任何情况下，作者或版权持有人都不对任何索赔、损害或其他责任负责，无论这些追责来自合同、侵权或其它行为中，
// 还是产生于、源于或有关于本软件以及本软件的使用或其它处置。

using System.Text.RegularExpressions;

namespace Furion.DataValidation;

/// <summary>
/// 验证项元数据
/// </summary>
[SuppressSniffer, AttributeUsage(AttributeTargets.Field)]
public class ValidationItemMetadataAttribute : Attribute
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="regularExpression">正则表达式</param>
    /// <param name="defaultErrorMessage">失败提示默认消息</param>
    /// <param name="regexOptions">正则表达式匹配选项</param>
    public ValidationItemMetadataAttribute(string regularExpression, string defaultErrorMessage, RegexOptions regexOptions = RegexOptions.None)
    {
        RegularExpression = regularExpression;
        DefaultErrorMessage = defaultErrorMessage;
        RegexOptions = regexOptions;
    }

    /// <summary>
    /// 正则表达式
    /// </summary>
    public string RegularExpression { get; set; }

    /// <summary>
    /// 默认验证失败类型
    /// </summary>
    public string DefaultErrorMessage { get; set; }

    /// <summary>
    /// 正则表达式选项
    /// </summary>
    public RegexOptions RegexOptions { get; set; }
}