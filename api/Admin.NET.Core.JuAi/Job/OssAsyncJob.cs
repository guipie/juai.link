// 麻省理工学院许可证
//
// 版权所有 (c) 2021-2023 zuohuaijun，大名科技（天津）有限公司  联系电话/微信：18020030720  QQ：515096995
//
// 特此免费授予获得本软件的任何人以处理本软件的权利，但须遵守以下条件：在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。


using Admin.NET.Core.JuAI.Service;
namespace Admin.NET.Core.JuAI;

/// <summary>
/// 同步chatgpt绘画图片到OSS
/// </summary>
[JobDetail("job_ossAsync", Description = "同步绘画图片到OSS", GroupName = "app", Concurrent = false)]
[HourlyAt(1, 30, TriggerId = "trigger_ossAsync", Description = "同步绘画图片到OSS", RunOnStart = true)]
public class OssAsyncJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;

    public OssAsyncJob(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        try
        {
            var originColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("【" + DateTime.Now + "】服务开始执行chatgpt image的同步");
            Console.ForegroundColor = originColor;

            using var serviceScope = _scopeFactory.CreateScope();

            var juFileService = serviceScope.ServiceProvider.GetService<JuFileService>();

            await juFileService!.GPTImageAsyncToOss("drawing/" + DateTime.Now.Year.ToString());
        }
        catch (Exception ex)
        {
            var originColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("【" + DateTime.Now + "】 chatgpt image服务执行失败,失败原因：" + ex.Message);
            Console.ForegroundColor = originColor;
        }
    }
}