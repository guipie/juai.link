//// 麻省理工学院许可证
////
//// 版权所有 (c) 2021-2023 zuohuaijun，大名科技（天津）有限公司  联系电话/微信：18020030720  QQ：515096995
////
//// 特此免费授予获得本软件的任何人以处理本软件的权利，但须遵守以下条件：在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
////
//// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
//// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。


//using Admin.NET.Core.JuAI.Service;
//using Furion.Logging;
//using Microsoft.Extensions.Logging;

//namespace Admin.NET.Core.JuAI;

///// <summary>
///// 测试任务
///// </summary>
//[JobDetail("job_test", Description = "测试任务", GroupName = "app", Concurrent = false)]
//[MinutelyAt(1, 59, TriggerId = "trigger_test", Description = "测试任务", MaxNumberOfRuns = 0, RunOnStart = true)]
//public class DemoTestJob : IJob
//{
//    private readonly IServiceScopeFactory _scopeFactory;

//    public DemoTestJob(IServiceScopeFactory scopeFactory)
//    {
//        _scopeFactory = scopeFactory;
//    }

//    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
//    {
//        try
//        {
//            var originColor = Console.ForegroundColor;
//            Console.ForegroundColor = ConsoleColor.Green;

//            Console.WriteLine("【" + DateTime.Now + "】测试服务开始执行");
//            Console.ForegroundColor = originColor;
//            using var serviceScope = _scopeFactory.CreateScope();

//            var logger = serviceScope.ServiceProvider.GetService<ILogger<DemoTestJob>>();
//            logger!.LogInformation("【" + DateTime.Now + "】测试任务是否插入");
//            await Task.Delay(1000, stoppingToken);
//        }
//        catch (Exception ex)
//        {
//            var originColor = Console.ForegroundColor;
//            Console.ForegroundColor = ConsoleColor.Red;
//            Console.WriteLine("【" + DateTime.Now + "】测试服务开始执行,失败原因：" + ex.Message);
//            Console.ForegroundColor = originColor;
//        }
//    }


//}