// MIT 许可证
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

using Furion;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// 应用中间件拓展类（由框架内部调用）
/// </summary>
[SuppressSniffer]
public static class AppApplicationBuilderExtensions
{
    /// <summary>
    /// 注入基础中间件（带Swagger）
    /// </summary>
    /// <param name="app"></param>
    /// <param name="routePrefix">空字符串将为首页</param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseInject(this IApplicationBuilder app, string routePrefix = default, Action<UseInjectOptions> configure = null)
    {
        // 载入中间件配置选项
        var configureOptions = new UseInjectOptions();
        configure?.Invoke(configureOptions);

        app.UseSpecificationDocuments(routePrefix, UseInjectOptions.SwaggerConfigure, UseInjectOptions.SwaggerUIConfigure);

        return app;
    }

    /// <summary>
    /// 注入基础中间件（带Swagger）
    /// </summary>
    /// <param name="app"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseInject(this IApplicationBuilder app, Action<UseInjectOptions> configure)
    {
        return app.UseInject(default, configure: configure);
    }

    /// <summary>
    /// 注入基础中间件
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseInjectBase(this IApplicationBuilder app)
    {
        return app;
    }

    /// <summary>
    /// 解决 .NET6 WebApplication 模式下二级虚拟目录错误问题
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder MapRouteControllers(this IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        return app;
    }

    /// <summary>
    /// 启用 Body 重复读功能
    /// </summary>
    /// <remarks>须在 app.UseRouting() 之前注册</remarks>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder EnableBuffering(this IApplicationBuilder app)
    {
        return app.Use(next => context =>
        {
            context.Request.EnableBuffering();
            return next(context);
        });
    }

    /// <summary>
    /// 添加应用中间件
    /// </summary>
    /// <param name="app">应用构建器</param>
    /// <param name="configure">应用配置</param>
    /// <returns>应用构建器</returns>
    internal static IApplicationBuilder UseApp(this IApplicationBuilder app, Action<IApplicationBuilder> configure = null)
    {
        // 调用自定义服务
        configure?.Invoke(app);
        return app;
    }
}