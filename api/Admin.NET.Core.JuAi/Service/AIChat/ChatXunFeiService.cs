// 麻省理工学院许可证
//
// 版权所有 (c) 2021-2023 zuohuaijun，大名科技（天津）有限公司  联系电话/微信：18020030720  QQ：515096995
//
// 特此免费授予获得本软件的任何人以处理本软件的权利，但须遵守以下条件：在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using Admin.NET.Core.JuAI.Service.AI;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
using NewLife.Serialization;
using Newtonsoft.Json.Linq;
using OpenAI.ObjectModels;
using System.Net.WebSockets;
using System.Text;
using Yitter.IdGenerator;

namespace Admin.NET.Core.JuAI.Service;
public class ChatXunFeiService : AppBaseService, IAiChatService
{

    private readonly XunfeiOptions _xunfeiOptions;
    private readonly UserManager _userManager;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ChatgptService _chatgptService;
    private readonly UserTokenService _userTokenService;
    public ChatXunFeiService(IOptions<XunfeiOptions> options, UserManager userManager,
        ChatgptService chatgptService,
        IHttpContextAccessor contextAccessor,
        UserTokenService userTokenService)
    {
        _xunfeiOptions = options.Value;
        _userManager = userManager;
        _chatgptService = chatgptService;
        _contextAccessor = contextAccessor;
        _userTokenService = userTokenService;
    }

    [TypeFilter(typeof(TokenUsageFilter), Arguments = new object[] { TokenUsageType.Chat })]
    [NonUnify]
    public async Task ChatAsync([FromBody] ChatPromptReq chatPrompts)
    {
        ChatGPT chatGPT = new()
        {
            Model = chatPrompts.Model,
            Question = chatPrompts.Prompt,
            Id = chatPrompts.Options.ChatDbId ?? YitIdHelper.NextId(),
            ConversationId = chatPrompts.Options.ConversationId
        };
        string url = GetAuthUrl(chatPrompts.Model);
        var listMessage = await _chatgptService.GetConversationChatGpt(chatPrompts);
        using ClientWebSocket webSocket0 = new();
        try
        {
            await webSocket0.ConnectAsync(new Uri(url), _contextAccessor.HttpContext.RequestAborted);

            XfJsonRequest request = new()
            {
                header = new XfHeader()
                {
                    app_id = _xunfeiOptions.AppId,
                    uid = _userManager.UserId.ToString(),
                },
                parameter = new XfParameter()
                {
                    chat = new XfChat()
                    {
                        domain = chatPrompts.Model,//模型领域，默认为星火通用大模型
                        temperature = 0.5,//温度采样阈值，用于控制生成内容的随机性和多样性，值越大多样性越高；范围（0，1）
                        max_tokens = 1024,//生成内容的最大长度，范围（0，4096）
                    }
                },
                payload = new XfPayload()
                {
                    message = new XfMessage()
                    {
                        text = listMessage.Select(m => new XfContent() { content = m.Content, role = m.Role }).ToList(),
                    }
                }
            };
            string jsonString = JsonConvert.SerializeObject(request);
            //连接成功，开始发送数据  
            var frameData2 = Encoding.UTF8.GetBytes(jsonString.ToString());

            _ = webSocket0.SendAsync(new ArraySegment<byte>(frameData2), WebSocketMessageType.Text, true, _contextAccessor.HttpContext.RequestAborted);

            // 接收流式返回结果进行解析
            byte[] receiveBuffer = new byte[1024];
            WebSocketReceiveResult result = await webSocket0.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), _contextAccessor.HttpContext.RequestAborted);
            var firstChunk = true;
            while (!result.CloseStatus.HasValue)
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string receivedMessage = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
                    var xfChatRes = receivedMessage.ToJsonEntity<ChatXunFeiRes>();
                    if (0 == xfChatRes.Header.Code)
                    {
                        chatGPT.Answer += xfChatRes.Payload.Choices.Text.FirstOrDefault().Content;
                        var chatRes = new ChatRes(chatdbId: chatGPT.Id, chatGPT.Answer, StaticValues.ChatMessageRoles.Assistant, chatGPT.ConversationId);
                        if (chatGPT.Answer.IsNullOrWhiteSpace()) continue;
                        Console.WriteLine($"已接收到数据： {receivedMessage}");
                        await _contextAccessor.HttpContext!.Response.WriteAsync((firstChunk ? "" : "\n") + chatRes.ToJson());
                        firstChunk = false;
                        await _contextAccessor.HttpContext!.Response.Body.FlushAsync();
                        if (xfChatRes.Payload.Choices.Status == 2)
                        {
                            Console.WriteLine($"最后一帧： {receivedMessage}");
                            chatGPT.ReqNum = xfChatRes.Payload.Usage.Text.PromptTokens;
                            chatGPT.ResNum = xfChatRes.Payload.Usage.Text.CompletionTokens;
                            await _chatgptService.InsertUpdateChatGpt(chatGPT);
                            await _userTokenService.SetMinusUserToken(chatGPT.ReqNum + chatGPT.ResNum);
                            return;
                        }
                    }
                    else
                    {
                        await _contextAccessor.HttpContext!.Response.WriteAsync((firstChunk ? "" : "\n") + new ChatRes(chatGPT.Id, $"出错了,请重试（联系vx:15100305）;错误信息：{receivedMessage}", StaticValues.ChatMessageRoles.Assistant, chatGPT.ConversationId).ToJson());
                        firstChunk = false;
                        await _contextAccessor.HttpContext!.Response.Body.FlushAsync();
                        return;
                    }

                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _contextAccessor.HttpContext!.Response.WriteAsync((firstChunk ? "" : "\n") + new ChatRes(chatGPT.Id, $"讯飞服务连接关闭,请重试;", StaticValues.ChatMessageRoles.Assistant, chatGPT.ConversationId).ToJson());
                    firstChunk = false;
                    await _contextAccessor.HttpContext!.Response.Body.FlushAsync();
                    return;
                }
                result = await webSocket0.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), _contextAccessor.HttpContext.RequestAborted);
            }
        }
        catch (Exception e)
        {
            await _contextAccessor.HttpContext!.Response.WriteAsync(new ChatRes(chatGPT.Id, $"出错了,请重试（联系vx:15100305）;错误信息：{e.Message}", StaticValues.ChatMessageRoles.Assistant, chatGPT.ConversationId).ToJson());
            await _contextAccessor.HttpContext!.Response.Body.FlushAsync();
            Log.CreateLogger<DrawingOpenAIService>().LogError("讯飞聊天出错,错误信息:" + e.Message, chatPrompts.ToJson(), e.StackTrace);
            return;
        }
    }
    // 返回code为错误码时，请查询https://www.xfyun.cn/document/error-code解决方案
    [NonAction]
    private string GetAuthUrl(string model)
    {
        string date = DateTime.UtcNow.ToString("r");
        Uri uri = new Uri("https://spark-api.xf-yun.com/v1.1/chat");
        if (model == "generalv2")
            uri = new Uri("https://spark-api.xf-yun.com/v2.1/chat");
        else if (model == "generalv3")
            uri = new Uri("https://spark-api.xf-yun.com/v3.1/chat");
        StringBuilder builder = new StringBuilder("host: ").Append(uri.Host).Append("\n").//
                                Append("date: ").Append(date).Append("\n").//
                                Append("GET ").Append(uri.LocalPath).Append(" HTTP/1.1");

        string sha = HMACsha256(_xunfeiOptions.ApiSecret, builder.ToString());
        string authorization = string.Format("api_key=\"{0}\", algorithm=\"{1}\", headers=\"{2}\", signature=\"{3}\"", _xunfeiOptions.ApiKey, "hmac-sha256", "host date request-line", sha);
        //System.Web.HttpUtility.UrlEncode

        string NewUrl = "https://" + uri.Host + uri.LocalPath;

        string path1 = "authorization" + "=" + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(authorization));
        date = date.Replace(" ", "%20").Replace(":", "%3A").Replace(",", "%2C");
        string path2 = "date" + "=" + date;
        string path3 = "host" + "=" + uri.Host;
        NewUrl = NewUrl + "?" + path1 + "&" + path2 + "&" + path3;
        return NewUrl.Replace("http://", "ws://").Replace("https://", "wss://");
    }
    public static string HMACsha256(string apiSecretIsKey, string buider)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(apiSecretIsKey);
        System.Security.Cryptography.HMACSHA256 hMACSHA256 = new System.Security.Cryptography.HMACSHA256(bytes);
        byte[] date = System.Text.Encoding.UTF8.GetBytes(buider);
        date = hMACSHA256.ComputeHash(date);
        hMACSHA256.Clear();

        return Convert.ToBase64String(date);

    }
}
