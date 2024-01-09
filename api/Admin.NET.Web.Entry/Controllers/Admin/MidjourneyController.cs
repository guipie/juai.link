using Admin.NET.Application;
using Admin.NET.Application.Const;
using Admin.NET.Application.JuAI;
using Admin.NET.Core;
using Admin.NET.Core.Service;
using Flurl.Util;
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
    [ApiDescriptionSettings(ApplicationAppConst.GroupName, Order = 101)]
    public class AdminMidController : AdminBaseController
    {
        private readonly ILogger<AdminMidController> logger;
        private readonly MidController _midController;
        public AdminMidController(MidController midController)
        {
            _midController = midController;
        }

        [HttpPost("fetchToDb")]
        public async Task<int> FetchToDb()
        {
            return await _midController.PostMidUrlsToDbAsync();
        }

        [HttpPost("oss/sync/{id}")]
        public async Task<(bool, string)> PostMidUrlToQiniuAsync([FromRoute] string id)
        {
            return await _midController.PostMidUrlToQiniuAsync(id);
        }
        [HttpPost("oss/sync/batch")]
        public async Task<string> PostMidUrslToQiniuAsync([FromBody] string[] ids)
        {
            return await _midController.PostMidUrslToQiniuAsync(ids);
        }
    }
}