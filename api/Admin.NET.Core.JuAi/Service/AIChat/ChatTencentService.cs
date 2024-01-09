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
using TencentCloud.Common;
using TencentCloud.Hunyuan.V20230901;
using TencentCloud.Hunyuan.V20230901.Models;
using Message = TencentCloud.Hunyuan.V20230901.Models.Message;
using TencentCloud.Common.Profile;
using Admin.NET.Core.JuAI.Service.AI.Tencent.Dto;

namespace Admin.NET.Core.JuAI.Service;
public class ChatTencentService : AppBaseService, IAiChatService
{

    private readonly TencentOptions _tencentOptions;
    private readonly UserManager _userManager;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ChatgptService _chatgptService;
    private readonly UserTokenService _userTokenService;
    private readonly ICache _cache;
    private readonly Credential cred;
    private readonly HunyuanClient _hunyuanClient;
    public ChatTencentService(IOptions<TencentOptions> options, UserManager userManager,
        ChatgptService chatgptService,
        IHttpContextAccessor contextAccessor,
        UserTokenService userTokenService, ICache cache)
    {
        _tencentOptions = options.Value;
        _userManager = userManager;
        _chatgptService = chatgptService;
        _contextAccessor = contextAccessor;
        _userTokenService = userTokenService;
        _cache = cache;
        cred = new Credential
        {
            SecretId = _tencentOptions.SecretId,
            SecretKey = _tencentOptions.SecretKey
        };
        _hunyuanClient = new HunyuanClient(cred, "ap-guangzhou");
    }

    [TypeFilter(typeof(TokenUsageFilter), Arguments = new object[] { TokenUsageType.Chat })]
    [NonUnify]
    public async Task ChatAsync([FromBody] ChatPromptReq chatPrompts)
    {

        // 为了保护密钥安全，建议将密钥设置在环境变量中或者配置文件中。
        // 硬编码密钥到代码中有可能随代码泄露而暴露，有安全隐患，并不推荐。
        // 这里采用的是从环境变量读取的方式，需要在环境变量中先设置这两个值。

        _contextAccessor.HttpContext!.Response.ContentType = "text/event-stream";
        var firstChunk = true;
        ChatGPT chatGPT = new()
        {
            Model = chatPrompts.Model,
            Question = chatPrompts.Prompt,
            Id = chatPrompts.Options.ChatDbId ?? YitIdHelper.NextId(),
            ConversationId = chatPrompts.Options.ConversationId
        };
        var listMessage = await _chatgptService.GetConversationChatGpt(chatPrompts);
        var chatRes = new ChatRes(chatGPT.Id, $"", StaticValues.ChatMessageRoles.Assistant, chatGPT.ConversationId);
        try
        {
            if (listMessage.Count > 0 && listMessage.Count % 2 == 0) listMessage.RemoveAt(0);
            var messages = listMessage.Select(m => new { role = m.Role, content = m.Content.IsNullOrEmpty() ? "我回答不了" : m.Content }).ToList();
            var req = new ChatStdRequest
            {
                Messages = messages.Select(m => new Message
                {
                    Role = m.role,
                    Content = m.content,
                }).ToArray()
            };
            ChatStdResponse resp = _hunyuanClient.ChatStdSync(req);
            foreach (var e in resp)
            {
                if (e.Data.IsNullOrEmpty()) continue;
                var readResult = JSON.Deserialize<ChatTencentRes>(e.Data);
                if (readResult == null || readResult.Choices == null || readResult.Choices.Length == 0)
                {
                    chatRes.text = $"会话出错,请重试（联系vx:15100305）;错误信息：{e.Data}";
                }
                else
                {
                    chatRes.text += readResult.Choices.FirstOrDefault().Delta.Content;
                    chatGPT.Answer = chatRes.text;
                    chatGPT.ReqNum = readResult.Usage.PromptTokens;
                    chatGPT.ResNum = readResult.Usage.CompletionTokens;
                }
                await _contextAccessor.HttpContext!.Response.WriteAsync((firstChunk ? "" : "\n") + chatRes.ToJson());
                await _contextAccessor.HttpContext!.Response.Body.FlushAsync();
                firstChunk = false;
                if (readResult.Choices.FirstOrDefault().FinishReason == "stop ") return;
            }

        }
        catch (Exception e)
        {
            chatRes.text = $"出错了,请重试（联系vx:15100305）;错误信息：{e.Message}";
            await _contextAccessor.HttpContext!.Response.WriteAsync((firstChunk ? "" : "\n") + chatRes.ToJson());
            await _contextAccessor.HttpContext!.Response.Body.FlushAsync();
            Log.CreateLogger<DrawingOpenAIService>().LogError("腾讯聊天出错,错误信息:" + e.Message, chatPrompts.ToJson(), e.StackTrace);
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
    [TypeFilter(typeof(TokenUsageFilter), Arguments = new object[] { TokenUsageType.Chat })]
    [NonUnify]
    public async Task ChatProAsync([FromBody] ChatPromptReq chatPrompts)
    {

        // 为了保护密钥安全，建议将密钥设置在环境变量中或者配置文件中。
        // 硬编码密钥到代码中有可能随代码泄露而暴露，有安全隐患，并不推荐。
        // 这里采用的是从环境变量读取的方式，需要在环境变量中先设置这两个值。

        _contextAccessor.HttpContext!.Response.ContentType = "text/event-stream";
        var firstChunk = true;
        ChatGPT chatGPT = new()
        {
            Model = chatPrompts.Model,
            Question = chatPrompts.Prompt,
            Id = chatPrompts.Options.ChatDbId ?? YitIdHelper.NextId(),
            ConversationId = chatPrompts.Options.ConversationId
        };
        var listMessage = await _chatgptService.GetConversationChatGpt(chatPrompts);
        var chatRes = new ChatRes(chatGPT.Id, $"", StaticValues.ChatMessageRoles.Assistant, chatGPT.ConversationId);
        try
        {
            if (listMessage.Count > 0 && listMessage.Count % 2 == 0) listMessage.RemoveAt(0);
            var messages = listMessage.Select(m => new { role = m.Role, content = m.Content.IsNullOrEmpty() ? "我回答不了" : m.Content }).ToList();
            var req = new ChatStdRequest
            {
                Messages = messages.Select(m => new Message
                {
                    Role = m.role,
                    Content = m.content,
                }).ToArray()
            };
            var reqPro = new ChatProRequest
            {
                Messages = messages.Select(m => new Message
                {
                    Role = m.role,
                    Content = m.content,
                }).ToArray()
            };
            ChatProResponse resp = _hunyuanClient.ChatProSync(reqPro);
            foreach (var e in resp)
            {
                if (e.Data.IsNullOrEmpty()) continue;
                var readResult = JSON.Deserialize<ChatTencentRes>(e.Data);
                if (readResult == null || readResult.Choices == null || readResult.Choices.Length == 0)
                {
                    chatRes.text = $"会话出错,请重试（联系vx:15100305）;错误信息：{e.Data}";
                }
                else
                {
                    chatRes.text += readResult.Choices.FirstOrDefault().Delta.Content;
                    chatGPT.Answer = chatRes.text;
                    chatGPT.ReqNum = readResult.Usage.PromptTokens;
                    chatGPT.ResNum = readResult.Usage.CompletionTokens;
                }
                await _contextAccessor.HttpContext!.Response.WriteAsync((firstChunk ? "" : "\n") + chatRes.ToJson());
                await _contextAccessor.HttpContext!.Response.Body.FlushAsync();
                firstChunk = false;
                if (readResult.Choices.FirstOrDefault().FinishReason == "stop ") return;
            }

        }
        catch (Exception e)
        {
            chatRes.text = $"出错了,请重试（联系vx:15100305）;错误信息：{e.Message}";
            await _contextAccessor.HttpContext!.Response.WriteAsync((firstChunk ? "" : "\n") + chatRes.ToJson());
            await _contextAccessor.HttpContext!.Response.Body.FlushAsync();
            Log.CreateLogger<DrawingOpenAIService>().LogError("腾讯聊天出错,错误信息:" + e.Message, chatPrompts.ToJson(), e.StackTrace);
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
}
