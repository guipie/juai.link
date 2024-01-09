

using Admin.NET.Core.JuAI.Service;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.Tokenizer.GPT3;

namespace Admin.NET.Core.JuAI;

[ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 1)]
public class GptsService(IOpenAIService openAIService, UserTokenService userTokenService) : AppBaseService
{
    private readonly IOpenAIService _openAIService = openAIService;
    private readonly UserTokenService _userTokenService = userTokenService;

    /// <summary>
    /// 翻译
    /// </summary>
    /// <param name="txt">翻译优化绘画的prompt文本</param>
    /// <returns></returns>
    [HttpPost("prompt/translation")]
    public async Task<string> TranslationPrompt([FromBody] string txt, bool isYh = false)
    {
        var prompt = "下面我让你来充当翻译家，你的目标是把任何语言翻译成英文，请翻译时不要带翻译腔，并且直接翻译，不要写解释，如果是英文直接返回，无需翻译。";
        if(isYh) { prompt += "需要注意的是你翻译的原文是AI绘画的Prompt，翻译后是用来做AI绘画，所以请尽可能的优化我的prompt，能够让AI生产更美观逼真的图片。"; }
        List<ChatMessage> messages =
        [
            ChatMessage.FromSystem(prompt),
            ChatMessage.FromUser(txt),
        ];
        var transChatgpt = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = messages,
            Model = Models.Gpt_4
        });
        if (transChatgpt.Successful)
        {
            var contents = transChatgpt.Choices.First().Message.Content;
            await _userTokenService.SetMinusUserToken(messages.Select(m => TokenizerGpt3.TokenCount(m.Content)).Sum() + TokenizerGpt3.TokenCount(contents));
            return contents;
        }
        else
        {
            throw Oops.Oh($"翻译出错了，错误信息：{transChatgpt.Error.Code}: {transChatgpt.Error.Message}");
        }
    }

}
