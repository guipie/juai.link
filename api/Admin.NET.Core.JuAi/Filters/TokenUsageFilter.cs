using Admin.NET.Core;
using Admin.NET.Core.JuAI.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Net;

namespace Admin.NET.Core.JuAI;

/// <summary>
/// 自定义ai值验证授权筛选器
/// </summary>
//[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
[SuppressSniffer]
public class TokenUsageFilter : IActionFilter
{

    private readonly UserTokenService _userTokenService;
    private readonly TokenUsageType _checkType;
    public TokenUsageFilter(TokenUsageType type, UserTokenService userTokenService)
    {
        _checkType = type;
        _userTokenService = userTokenService;
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        Console.WriteLine($"{_checkType}执行结束,当前消耗[{22}]token");
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var num = _userTokenService.GetUserToken();
        Console.WriteLine($"{_checkType}执行开始,当前用户token：{num}");
        if (_checkType == TokenUsageType.Chat && num < 100)
        {
            throw Oops.Oh($"AI值不足,请去累积");
        }
        else if (_checkType == TokenUsageType.ChatImage && num < 20000)
        {
            throw Oops.Oh($"GPT绘画失败,AI值不足,请去累积");
        }
        else if (_checkType == TokenUsageType.Midjourney && num < 15000)
        {
            throw Oops.Oh($"Midjourney绘画失败,AI值不足,请去累积");
        }
    }

}