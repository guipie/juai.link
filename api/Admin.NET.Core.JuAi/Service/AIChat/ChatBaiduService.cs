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
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System.IO;
using COSXML.Network;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.VisualBasic;
using Elasticsearch.Net;
using static SKIT.FlurlHttpClient.Wechat.Api.Models.CgibinComponentApiGetAuthorizerInfoResponse.Types;
using System.Net.WebSockets;
using NewLife.Http;
using WebSocketMessageType = System.Net.WebSockets.WebSocketMessageType;
using Admin.NET.Core.JuAI.Entity;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace Admin.NET.Core.JuAI.Service;
public class ChatBaiduService : AppBaseService, IAiChatService
{

    private readonly BaiduOptions _baiduOptions;
    private readonly UserManager _userManager;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ChatgptService _chatgptService;
    private readonly UserTokenService _userTokenService;
    private readonly ICache _cache;
    public ChatBaiduService(IOptions<BaiduOptions> options, UserManager userManager,
        ChatgptService chatgptService,
        IHttpContextAccessor contextAccessor,
        UserTokenService userTokenService, ICache cache)
    {
        _baiduOptions = options.Value;
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
        var chatUrl = $"{GetChatApiUrl(chatPrompts.Model)}?access_token={await GetAccessToken()}";
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
            var body = @"{""stream"":true,""user_id"":""" + _userManager.UserId + @""",""messages"":" + messages.ToJson() + "}";
            var webReq = WebRequest.Create(new Uri(chatUrl)) as HttpWebRequest;
            webReq.Method = "POST";
            webReq.ContentType = "application/json";
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
                var readResult = JSON.Deserialize<ChatBaiDuChatRes>(readStr);
                if (readResult != null && readResult.ErrorCode > 0 && !readResult.ErrorMsg.IsNullOrEmpty())
                {
                    chatRes.text = $"会话出错,请重试（联系vx:15100305）;错误信息：{readResult.ErrorMsg}";
                }
                else
                {
                    chatRes.text += readResult.Result;
                    chatGPT.Answer = chatRes.text;
                    chatGPT.ReqNum = readResult.Usage.PromptTokens;
                    chatGPT.ResNum = readResult.Usage.CompletionTokens;
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
            Log.CreateLogger<DrawingOpenAIService>().LogError("百度聊天出错,错误信息:" + e.Message, chatPrompts.ToJson(), e.StackTrace);
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

    /// <summary>
    /// 使用 AK，SK 生成鉴权签名（Access Token） 
    /// </summary>
    /// <returns>鉴权签名信息（Access Token）</returns>
    [NonAction]
    private async Task<string> GetAccessToken()
    {
        if (_cache.ContainsKey(AppCacheConst.chat_badu_access))
            return _cache.Get<string>(AppCacheConst.chat_badu_access);
        var accessTokenUrl = $"https://aip.baidubce.com/oauth/2.0/token?grant_type=client_credentials&client_id={_baiduOptions.ApiKey}&client_secret={_baiduOptions.ApiSecret}";
        var accessStr = (await accessTokenUrl.PostAsStringAsync());
        var accessEntity = accessStr.ToJsonEntity<ChatBaiDuAccessReq>();
        _cache.Set(AppCacheConst.chat_badu_access, accessEntity.access_token, accessEntity.expires_in - 20);
        return accessEntity.access_token;
    }
    private string GetChatApiUrl(string model)
    {
        switch (model)
        {
            case "model_ernie_bot_turbo":
                return "https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/eb-instant";
            case "model_ernie_bot_4":
                return "https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/completions_pro";
            case "model_badiu_llama2_70b":
                return "https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/llama_2_7b";
            case "model_baidu_llama2_13b":
                return "https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/llama_2_13b";
            case "model_baidu_llama2_7b_cn":
                return "https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/qianfan_chinese_llama_2_7b";
            case "model_baidu_chatglm2_6b_32k":
                return "https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/chatglm2_6b_32k";
            case "model_baidu_aquila_chat7b":
                return "https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/aquilachat_7b";
            case ":model_baidu_bloomz_7b":
                return "https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/bloomz_7b1";
            default:
                return "https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/completions";
        }
    }
}
