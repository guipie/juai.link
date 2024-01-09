using Admin.NET.Application;
using Admin.NET.Application.JuAI;
using Admin.NET.Core;
using Admin.NET.Core.JuAI.Service;
using Admin.NET.Core.Service;
using Flurl.Http;
using Flurl.Util;
using Furion.DependencyInjection;
using Furion.FriendlyException;
using Furion.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Nest;
using OnceMi.AspNetCore.OSS;
using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;
using System.ComponentModel;

namespace Admin.NET.Web.Entry.Controllers
{
    [ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 1)]
    public class MidController : AppBaseController, IScoped
    {
        private readonly Dictionary<string, string> headDic = new() { ["mj-api-secret"] = "juai.link" };
        private static readonly string midDomain = App.GetOptions<MidjourneyOptions>().ApiDomain;
        private static readonly string noticeBackUrl = App.GetOptions<MidjourneyOptions>().NotifyHookUrl;

        private readonly UserManager _userManager;
        private readonly MidjourneyService _midjourneyService;
        private readonly UserTokenService _userTokenService;

        private readonly IHubContext<OnlineUserHub, IOnlineUserHub> _chatHubContext;
        private readonly SysCacheService _sysCacheService;
        private readonly OSSProviderOptions _OSSProviderOptions;
        private readonly IOSSService _OSSService;
        public MidController(UserManager userManager, MidjourneyService midjourneyService, IHubContext<OnlineUserHub, IOnlineUserHub> chatHubContext, SysCacheService sysCacheService,
            UserTokenService userTokenService, IOptions<OSSProviderOptions> oSSProviderOptions, IOSSServiceFactory ossServiceFactory)
        {
            _userManager = userManager;
            _midjourneyService = midjourneyService;
            _chatHubContext = chatHubContext;
            _sysCacheService = sysCacheService;
            _userTokenService = userTokenService;

            _OSSProviderOptions = oSSProviderOptions.Value;
            if (_OSSProviderOptions.IsEnable)
                _OSSService = ossServiceFactory.Create(Enum.GetName(_OSSProviderOptions.Provider));
        }
        [HttpGet("image/{id}")]
        [AllowAnonymous]
        public async Task<MidjourneyTaskRes> GetImage([FromRoute] string id)
        {
            return await _midjourneyService.GetShareImage(id);
        }

        [HttpGet("newest")]
        [AllowAnonymous]
        public async Task<IList<MidjourneyNotify>> GetNewest(long? submitTime)
        {
            return await _midjourneyService.GetNewest(submitTime);
        }
        [HttpGet("recommend")]
        [AllowAnonymous]
        public async Task<IList<MidjourneyNotify>> GetRecommend(long? submitTime)
        {
            return await _midjourneyService.GetRecommend(submitTime);
        }
        [NonAction]
        private async Task<T> GetMidFunc<T>(string url, string method = "get", object body = null)
        {

            var res = (midDomain + url).SetHeaders(headDic);
            if (body != null) { res.SetBody(body, "application/json"); }
            if (method == "get")
                return JSON.Deserialize<T>(await res.GetAsStringAsync());
            else if (method == "post")
                return JSON.Deserialize<T>(await res.PostAsStringAsync());
            else if (method == "put")
                return JSON.Deserialize<T>(await res.PutAsStringAsync());
            else
                throw new Exception("请求出错了");
        }
        ///// <summary>
        ///// 获取所有正在进行的任务
        ///// </summary>
        ///// <returns></returns>

        //[HttpGet("tasks")]
        //public async Task<MidjourneyTaskRes> AllTask()
        //{
        //    return await GetMidFunc<MidjourneyTaskRes>("/mj/task/list");
        //}
        ///// <summary>
        ///// 获取我的所有代理任务
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("tasks/my/proxy")]
        //public async Task<MidjourneyTaskRes> MyTaskAsync()
        //{
        //    var ids = await _midjourneyService.GetMyTaskIds();
        //    if (ids.Length > 0)
        //        return await GetMidFunc<MidjourneyTaskRes>("/mj/task/list-by-condition", "post", new { ids });
        //    return null;
        //}
        [HttpGet("tasks/my/db")]
        public async Task<IList<MidjourneyTaskRes>> MyTasksByDbAsync(DateTime? createTime)
        {
            return await _midjourneyService.GetMyNotifyTasks(createTime);
        }
        [HttpDelete("task/{id}")]
        public async Task<bool> DelTaskById([FromRoute] string id)
        {
            return await _midjourneyService.DelTaskById(id);
        }
        /// <summary>
        /// 获取我的所有任务
        /// </summary>
        /// <returns></returns>
        [HttpGet("tasks/{ids}")]
        public async Task<MidjourneyTaskRes> TaskByIdsAsync([FromRoute] string[] ids)
        {
            return await GetMidFunc<MidjourneyTaskRes>("/mj/task/list-by-condition", "post", new { ids });
        }
        [HttpGet("task/fetch/{id}")]
        public async Task<MidjourneyTaskRes> TaskByIdAsync([FromRoute] string id)
        {
            var taskRes = await GetMidFunc<MidjourneyTaskRes>($"/mj/task/{id}/fetch");
            if (taskRes.Successed)
            {
                if (taskRes.State.IsNullOrEmpty()) taskRes.State = _userManager.UserId.ToString();
                await _midjourneyService.PostNotifyInfo(taskRes);
            }
            var task = await _midjourneyService.GetTaskByTaskId(id);
            if (task != null)
            {
                taskRes.Prompt = task.Prompt;
                taskRes.PromptEn = task.PromptEn;
            }
            return taskRes;
        }

        /// <summary>
        /// 任务回调地址
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("notify/hook"), AllowAnonymous]
        public async Task<dynamic> PostNotify([FromBody] MidjourneyTaskRes req)
        {
            var user = _sysCacheService.Get<SysOnlineUser>(CacheConst.KeyUserOnline + req.State);
            if (user != null)
            {
                var input = new MessageInput() { Message = req.ToJson(), MessageType = MessageTypeEnum.Success, Title = "midjourynery", UserId = user.UserId };
                await _chatHubContext.Clients.Client(user.ConnectionId).ReceiveMessage(input);
            }
            return await _midjourneyService.PostNotifyInfo(req);
        }
        #region 所有提交任务相关

        /// <summary>
        /// 提交绘画任务
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>

        [TypeFilter(typeof(TokenUsageFilter), Arguments = new object[] { TokenUsageType.Midjourney })]
        [HttpPost("submit/imagine")]
        public async Task<ImagineTaskRes> PostImagineTask([FromBody] ImagineTaskReq req)
        {
            if (req.prompt.IsNullOrEmpty()) throw Oops.Bah("输入生成图片描述");
            string promptOrgin = req.prompt;
            if (req.promptEn.Length > 0) req.prompt = req.promptEn;
            req.state = _userManager.UserId.ToString();
            req.notifyHook = noticeBackUrl;
            ImagineTaskRes drawingResult = await GetMidFunc<ImagineTaskRes>("/mj/submit/imagine", "post", req);
            if (drawingResult.code == "1")
            {
                await _userTokenService.SetMinusUserToken(19000);
                await _midjourneyService.PostTask(drawingResult.result, promptOrgin, req.promptEn);
            }
            drawingResult.prompt = promptOrgin;
            drawingResult.promptEn = req.promptEn;
            return drawingResult;
        }
        /// <summary>
        /// 提交绘画变化任务 u-v指令
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [TypeFilter(typeof(TokenUsageFilter), Arguments = new object[] { TokenUsageType.Midjourney })]
        [HttpPost("submit/change")]
        public async Task<ImagineTaskRes> PostImagineTask([FromBody] ChangeTaskReq req)
        {
            if (req.action.IsNullOrEmpty() || req.index < 0 || req.taskId.IsNullOrEmpty()) throw Oops.Bah("输入参数不完整");
            if (_midjourneyService.IsChanged(req.index, req.action, req.taskId)) throw Oops.Bah($"已经提交过【{req.action + req.index}】，无需重复提交");
            req.state = _userManager.UserId.ToString();
            req.notifyHook = noticeBackUrl;
            var drawingResult = await GetMidFunc<ImagineTaskRes>("/mj/submit/change", "post", req);
            if (drawingResult.code == "1")
            {
                await _userTokenService.SetMinusUserToken(19000);
                var relationTask = await _midjourneyService.GetTaskByTaskId(req.taskId);
                await _midjourneyService.PostTask(drawingResult.result, relationTask.Prompt, relationTask.PromptEn, req.taskId);
            }
            return drawingResult;
        }
        #endregion

        #region 同步mid图片到七牛云

        [DisplayName("同步单个任务到到七牛云"), HttpPost("oss/sync")]
        public async Task<(bool, string)> PostMidUrlToQiniuAsync([FromBody] string id)
        {
            MidjourneyNotify taskNotice = await _midjourneyService.GetNoticeByTaskId(id);
            if (taskNotice.ImageUrl.IsNullOrEmpty()) Oops.Bah("未找到此任务，无法云同步");
            if (!taskNotice.OssUrl.IsNullOrEmpty()) Oops.Bah("已经云同步过，无需重复");
            Config config = new()
            {
                Zone = Zone.ZONE_CN_South
            };
            Mac mac = new(_OSSProviderOptions.AccessKey, _OSSProviderOptions.SecretKey);
            BucketManager bucketManager = new(mac, config);
            var bFileName = "Mid/" + taskNotice.ImageUrl.Split("/", StringSplitOptions.RemoveEmptyEntries).Last();
            HttpResult ret = bucketManager.Fetch(taskNotice.ImageUrl, _OSSProviderOptions.Bucket, bFileName);
            if (ret.Code != (int)HttpCode.OK)
            {
                Log.CreateLogger<QiNiuFileService>().LogError("同步文件出错" + ret.Code, ret.Text);
                var localImg = taskNotice.ImageUrl.GetStreamAsync().Result;
                var isStreamUpload = await _OSSService.PutObjectAsync(_OSSProviderOptions.Bucket, bFileName, localImg);
                if (isStreamUpload)
                {
                    var url = $"{(_OSSProviderOptions.IsEnableHttps ? "https" : "http")}://{_OSSProviderOptions.Endpoint}/{bFileName}";
                    await _midjourneyService.UpdateOssById(url, id);
                    return new(isStreamUpload, url);
                }
                else
                    return new(isStreamUpload, ret.Text + ret.RefText);
            }
            else
            {
                string qnUrl = "http://" + _OSSProviderOptions.Endpoint + "/" + bFileName;
                await _midjourneyService.UpdateOssById(qnUrl, id);
                return new(true, qnUrl);
            }
        }

        [DisplayName("同步所有未同步的任务到到七牛云"), HttpPost("oss/sync/batch")]
        public async Task<string> PostMidUrslToQiniuAsync([FromBody] string[] ids)
        {
            try
            {
                if (ids == null || ids.Length == 0)
                    ids = (await _midjourneyService.GetNoOssNoticeTask()).Select(m => m.Id).ToArray();
                foreach (string id in ids)
                    _ = PostMidUrlToQiniuAsync(id);
            }
            catch (Exception ex)
            {
                Log.CreateLogger<MidController>().LogError($"{ids}同步出错", ex.Message, ex.StackTrace, ex);
            }
            return "任务执行中";
        }

        [DisplayName("同步任务到DB"), HttpPost("fetchToDb")]
        public async Task<int> PostMidUrlsToDbAsync()
        {
            int num = 0;
            var allRemoteTasks = await GetMidFunc<IList<MidjourneyTaskRes>>("/mj/task/list", "get");
            foreach (var item in allRemoteTasks)
            {
                await _midjourneyService.UpdateOrInsertByTaskRes(item);
                num++;
            }
            return num;
            //var toFetchTask = allRemoteTasks.Where(m => m.Status == "SUCCESS" && !m.ImageUrl.IsNullOrEmpty());
            //var allLocalOssTasks = await _midjourneyService.GetOssNoticeTask(); 
        }
        #endregion  
    }
}