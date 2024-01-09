// 麻省理工学院许可证
//
// 版权所有 (c) 2021-2023 zuohuaijun，大名科技（天津）有限公司  联系电话/微信：18020030720  QQ：515096995
//
// 特此免费授予获得本软件的任何人以处理本软件的权利，但须遵守以下条件：在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using Admin.NET.Core.JuAI.Service.AI;
using Microsoft.Extensions.Options;
using OpenAI.ObjectModels;
using Yitter.IdGenerator;
using Furion.RemoteRequest.Extensions;
using NewLife.Serialization;
using System.Net;
using System.Text;
using static OpenAI.ObjectModels.Models;
using static SKIT.FlurlHttpClient.Wechat.Api.Models.CgibinExpressIntracityUpdateStoreRequest.Types;
using System.Linq;

namespace Admin.NET.Core.JuAI.Service;
public class ChatAlibabaService : AppBaseService, IAiChatService
{

    private readonly AlibabaOptions _alibabaOptions;
    private readonly UserManager _userManager;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ChatgptService _chatgptService;
    private readonly UserTokenService _userTokenService;
    private readonly ICache _cache;
    public ChatAlibabaService(IOptions<AlibabaOptions> options, UserManager userManager,
        ChatgptService chatgptService,
        IHttpContextAccessor contextAccessor,
        UserTokenService userTokenService, ICache cache)
    {
        _alibabaOptions = options.Value;
        _userManager = userManager;
        _chatgptService = chatgptService;
        _contextAccessor = contextAccessor;
        _userTokenService = userTokenService;
        _cache = cache;
    }

    [TypeFilter(typeof(TokenUsageFilter), Arguments = new object[] { TokenUsageType.Chat })]
    [NonUnify]
    public async Task ChatAsync([FromBody] ChatPromptReq chatPrompts)
    {
        _contextAccessor.HttpContext!.Response.ContentType = "text/event-stream";
        var firstChunk = true;
        var chatUrl = GetChatApiUrl(chatPrompts.Model);
        ChatGPT chatGPT = new()
        {
            Model = chatPrompts.Model,
            Question = chatPrompts.Prompt,
            Id = chatPrompts.Options.ChatDbId ?? YitIdHelper.NextId(),
            ConversationId = chatPrompts.Options.ConversationId
        };
        var chatRes = new ChatRes(chatGPT.Id, $"", StaticValues.ChatMessageRoles.Assistant, chatGPT.ConversationId);
        try
        {
            var body = await GetBodyPramsAsync(chatPrompts);
            var webReq = WebRequest.Create(new Uri(chatUrl)) as HttpWebRequest;
            webReq.Method = "POST";
            webReq.Headers.Add("Content-Type", "application/json");
            webReq.Headers.Add("Authorization", "Bearer " + _alibabaOptions.DashscopeKey);
            webReq.Headers.Add("X-DashScope-SSE", "enable");
            var sw = new StreamWriter(webReq.GetRequestStream());
            sw.Write(body);
            sw.Flush();
            sw.Close();
            var response = (HttpWebResponse)webReq.GetResponse();
            using var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            //var result = chatUrl.SetContentType("application/json").SetBody(body).PostAsStreamAsync(); 
            //// 使用StreamReader从文件流中读取数据
            //using StreamReader reader = new StreamReader(result.Result.Stream);
            while (!reader.EndOfStream)// 读取并输出文件内容直到文件结束
            {
                var readStr = reader.ReadLine();
                if (readStr.IsNullOrEmpty() || !readStr.StartsWith("data:")) continue;
                readStr = readStr.Replace("data:", "");
                var readResult = JSON.Deserialize<ChatAlibabaRes>(readStr);
                if (readResult.code.IsNullOrEmpty() && readResult.message.IsNullOrEmpty())
                {
                    if (chatPrompts.Model == "qwen-vl-plus")
                        chatRes.text = readResult.output.choices.FirstOrDefault().message.content.Select(m => m.text).FirstOrDefault();
                    else
                        chatRes.text = readResult.output.text;
                    chatGPT.Answer = chatRes.text;
                    chatGPT.ReqNum = readResult.usage.input_tokens;
                    chatGPT.ResNum = readResult.usage.output_tokens;
                }
                else
                {
                    chatRes.text = $"会话出错,请重试（联系vx:15100305）;错误信息：{readResult.code},{readResult.message}";
                }
                await _contextAccessor.HttpContext!.Response.WriteAsync((firstChunk ? "" : "\n") + chatRes.ToJson());
                await _contextAccessor.HttpContext!.Response.Body.FlushAsync();
                firstChunk = false;
            }

        }
        catch (Exception e)
        {
            chatRes.text = $"出错了,请重试（联系vx:15100305）;错误信息：{e.Message}";
            await _contextAccessor.HttpContext!.Response.WriteAsync((firstChunk ? "" : "\n") + chatRes.ToJson());
            await _contextAccessor.HttpContext!.Response.Body.FlushAsync();
            Log.CreateLogger<DrawingOpenAIService>().LogError("阿里巴巴聊天出错,错误信息:" + e.Message, chatPrompts.ToJson(), e.StackTrace);
            return;
        }
        finally
        {
            await _chatgptService.InsertUpdateChatGpt(chatGPT);
            await _userTokenService.SetMinusUserToken(chatGPT.ReqNum + chatGPT.ResNum);
            await _contextAccessor.HttpContext!.Response.Body.FlushAsync();
            await _contextAccessor.HttpContext!.Response.CompleteAsync().ConfigureAwait(false);
        }
    }
    private string GetChatApiUrl(string model)
    {
        switch (model)
        {
            case "qwen-vl-plus":
                return "https://dashscope.aliyuncs.com/api/v1/services/aigc/multimodal-generation/generation";
            default:
                return "https://dashscope.aliyuncs.com/api/v1/services/aigc/text-generation/generation";
        }
    }
    private async Task<string> GetBodyPramsAsync(ChatPromptReq chatPrompts)
    {
        var listMessage = await _chatgptService.GetConversationChatGpt(chatPrompts);
        var messages = listMessage.Select(m => new { role = m.Role, content = m.Content.IsNullOrEmpty() ? "我回答不了" : m.Content }).ToList();
        switch (chatPrompts.Model)
        {
            case "qwen-vl-plus":
                return new
                {
                    model = chatPrompts.Model,
                    input = new { messages = messages.Select(m => new { m.role, content = new List<TextRecord>() { new TextRecord(m.content) } }) },
                }.ToJson();
            default:
                return new
                {
                    model = chatPrompts.Model,
                    input = new { messages },
                }.ToJson();
        }
    }
}

internal record TextRecord(string Text);
