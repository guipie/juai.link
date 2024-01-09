

using Admin.NET.Core;
using Admin.NET.Core.JuAI.Service;
using Admin.NET.Core.Service;
using Furion.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Admin.NET.Web.Core;

/// <summary>
/// 自定义授权筛选器
/// </summary>
//[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class TokenAuthorizationHandler : IActionFilter
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {

        Console.WriteLine("token检查......");
        var service = context.GetCurrentHttpContext().RequestServices.GetService<UserTokenService>();
        Console.WriteLine($"token检测结束,剩余token:{service.GetUserToken()}");
        //context.Result = new JsonResult(new AdminResult<string>() { Code = 403, Message = "token余额不足,请去累积", Time = DateTime.Now });
        await Task.Delay(1000);
        context.Fail();
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        throw new NotImplementedException();
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        throw new NotImplementedException();
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var service = context.HttpContext.RequestServices.GetService<UserTokenService>();
        Console.WriteLine($"token检测结束,剩余token:{service.GetUserToken()}");
        await Task.Delay(1000);
    }
}