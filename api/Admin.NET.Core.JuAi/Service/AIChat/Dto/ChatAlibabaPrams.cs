// 麻省理工学院许可证
//
// 版权所有 (c) 2021-2023 zuohuaijun，大名科技（天津）有限公司  联系电话/微信：18020030720  QQ：515096995
//
// 特此免费授予获得本软件的任何人以处理本软件的权利，但须遵守以下条件：在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.NET.Core.JuAI.Service.AI.Dto;

class ChatAlibabaRes
{
    //调用结果信息，对于千问模型，包含输出text。
    public OutputClass output { get; set; }
    //计量信息，表示本次请求计量数据。
    public UsageClass usage { get; set; }
    //本次请求的系统唯一码
    public string request_id { get; set; }


    //错误代码
    public string code { get; set; }
    //错误信息
    public string message { get; set; }
}

/// <summary>
/// 调用结果信息，对于千问模型，包含输出text。
/// </summary>
class OutputClass
{
    //停止原因，null：生成过程中 stop：stop token导致结束 length：生成长度导致结束
    public string finish_reason { get; set; }
    //输出回答
    public string text { get; set; }
    //qwen-vl-plus 模型 输出回答方式
    public List<Message> choices { get; set; }
}
public class Message
{
    public MessageContent message { get; set; }
}
public class MessageContent
{
    public string role { get; set; }
    public List<MessageContentText> content { get; set; }
}
public partial class MessageContentText
{
    public string text { get; set; }
}
/// <summary>
/// 计量信息，表示本次请求计量数据。
/// </summary>
class UsageClass
{
    //总数
    public int total_tokens { get; set; }
    //模型生成回复转换为Token后的长度。
    public int output_tokens { get; set; }
    //用户输入文本转换成Token后的长度。
    public int input_tokens { get; set; }
}
