
using Flurl.Http;
using Furion.LinqBuilder;
using Furion.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnceMi.AspNetCore.OSS;
using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;
using System.Linq.Expressions;
using System.Web;
using Yitter.IdGenerator;

namespace Admin.NET.Core.JuAI.Service;
[ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 100)]
public class JuFileService : AppBaseService, ITransient
{
    private readonly SysFileService _sysFileService;

    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysFile> _sysFileRep;
    private readonly SqlSugarRepository<SysFileRelation> _sysFileRelationRep;
    private readonly SqlSugarRepository<ChatGPTImage> _chatGPTImageRep;
    private readonly OSSProviderOptions _OSSProviderOptions;
    private readonly UploadOptions _uploadOptions;
    private readonly IOSSService _OSSService;

    public JuFileService(UserManager userManager,
        SqlSugarRepository<SysFile> sysFileRep,
        IOptions<OSSProviderOptions> oSSProviderOptions,
        IOptions<UploadOptions> uploadOptions,
        IOSSServiceFactory ossServiceFactory,
        SysFileService sysFileService,
        SqlSugarRepository<SysFileRelation> sysFileRelationRep, SqlSugarRepository<ChatGPTImage> chatGPTImageRep
        )
    {
        _userManager = userManager;
        _sysFileRep = sysFileRep;
        _OSSProviderOptions = oSSProviderOptions.Value;
        _uploadOptions = uploadOptions.Value;
        _OSSService = ossServiceFactory.Create(Enum.GetName(_OSSProviderOptions.Provider));
        _sysFileService = sysFileService;
        _sysFileRelationRep = sysFileRelationRep;
        _chatGPTImageRep = chatGPTImageRep;
    }
    [DisplayName("上传文件")]
    public async Task<FileOutput> UploadFile([Required] IFormFile file, [FromQuery] string? path)
    {
        return await _sysFileService.UploadFile(file, path);
    }
    [DisplayName("删除文件")]
    public async Task DeleteFile([Required, FromRoute] long id)
    {
        await _sysFileService.DeleteFile(new DeleteFileInput() { Id = id });
    }
    [DisplayName("更新File的关联ID")]
    public async Task PostFileRelationId([Required, FromRoute] long[]? fileIds, long relationId, FileRelationEnum fileRelation)
    {
        await _sysFileRelationRep.DeleteAsync(m => m.RelationId == relationId);
        await _sysFileRelationRep.InsertRangeAsync(fileIds.Select(m => new SysFileRelation() { RelationId = relationId, FileId = m, FileRelationType = fileRelation }).ToList());
    }
    [DisplayName("添加文件到数据库")]
    public async Task<bool> AddFileToDb([Required, FromRoute] long relationId, string[] urls, string filePath, FileRelationEnum fileRelationEnum)
    {
        List<SysFile> files = new();
        foreach (var url in urls)
        {
            if (url.IsNullOrEmpty()) continue;
            Uri uri = new(url);
            var fileName = HttpUtility.UrlDecode(uri.Segments.Last()) ?? "error.jpg";
            files.Add(new SysFile()
            {
                Id = YitIdHelper.NextId(),
                BucketName = _OSSProviderOptions.IsEnable ? _OSSProviderOptions.Bucket : "Local",
                FileName = fileName,
                FilePath = filePath,
                Suffix = Path.GetExtension(fileName),
                Url = url,
            });
        }
        await PostFileRelationId(files.Select(m => m.Id).ToArray(), relationId, fileRelationEnum);
        return await _sysFileRep.InsertRangeAsync(files);
    }
    [NonAction]
    public async Task GPTImageAsyncToOss(string ossPath = "")
    {
       var needAsyncImges = await _sysFileRep.AsQueryable().Where(m => SqlFunc.IsNullOrEmpty(m.Provider) || SqlFunc.IsNullOrEmpty(m.FilePath)).ToListAsync();
        foreach (var item in needAsyncImges)
        {
            try
            {
                var file = await ImageAsync(item.FilePath.IsNullOrEmpty() ? ossPath : item.FilePath!, item);
                var relation = await _sysFileRelationRep.GetFirstAsync(m => m.FileId == file.Id);
                await _chatGPTImageRep.UpdateAsync(m => new ChatGPTImage() { OssUrl = file.Url }, m => m.Id == relation.RelationId);
            }
            catch (Exception ex)
            {
                string errmsg = "同步ChatgptImage任务出错," + ex.Message;
                Log.CreateLogger<JuFileService>().LogError(errmsg, ex.StackTrace);
                await Console.Error.WriteAsync(errmsg);
            }
        }
    }
    [NonAction]
    public async Task<SysFile> ImageAsync(string ossPath, SysFile item)
    {
        //if (!string.IsNullOrEmpty(item.Url))
        //{
        //    var file = await _sysFileRep.AsQueryable().FirstAsync(m => m.Url == item.Url || (m.FilePath == item.FilePath && m.FileName == item.FileName));
        //    if (file != null && file.Id > 0) return file;
        //}

        Uri uri = new(uriString: item.Url!);
        var path = (ossPath.EndsWith("/") ? ossPath : ossPath + "/");
        var filePathName = path + HttpUtility.UrlDecode(uri.Segments.Last());
        Config config = new()
        {
            Zone = Zone.ZONE_CN_South
        };
        Mac mac = new(_OSSProviderOptions.AccessKey, _OSSProviderOptions.SecretKey);
        BucketManager bucketManager = new(mac, config);
        HttpResult ret = bucketManager.Fetch(item.Url, _OSSProviderOptions.Bucket, filePathName);
        var uploadedUrl = string.Empty;
        if (ret.Code != (int)HttpCode.OK)
        {
            Log.CreateLogger<JuFileService>().LogError("Fetch同步文件出错" + ret.Code, ret.Text);
            var localImg = item.Url.GetStreamAsync().Result;
            var isStreamUpload = await _OSSService.PutObjectAsync(_OSSProviderOptions.Bucket, filePathName, localImg);
            if (isStreamUpload)
            {
                try
                {
                    uploadedUrl = $"{(_OSSProviderOptions.IsEnableHttps ? "https" : "http")}://{_OSSProviderOptions.Endpoint}/{filePathName}";
                    item.SizeKb = (localImg.Length / 1024).ToString();
                }
                catch (Exception)
                {
                }
            }
            else
                Log.CreateLogger<JuFileService>().LogError("PutObjectAsync同步文件出错" + ret.Code, ret.Text);
        }
        else
        {
            try
            {
                uploadedUrl = $"{(_OSSProviderOptions.IsEnableHttps ? "https" : "http")}://{_OSSProviderOptions.Endpoint}/{filePathName}";
                //item.SizeInfo = ret.Text;
                item.SizeKb = (JSON.Deserialize<Dictionary<string, string>>(ret.Text).GetValueOrDefault("fsize").ToLong() / 1024).ToString();
            }
            catch (Exception)
            {

            }
        }
        if (uploadedUrl.IsNullOrEmpty()) return item;
        item.BucketName = _OSSProviderOptions.Bucket;
        item.Provider = _OSSProviderOptions.Provider.ToString();
        item.FilePath = filePathName;
        item.Url = uploadedUrl ?? item.Url;
        item.FileName = Path.GetFileName(uploadedUrl);
        item.Suffix = Path.GetExtension(uploadedUrl);
        item.FileMd5 = item.Url;
        await _sysFileRep.InsertOrUpdateAsync(item);
        return item;
    }

    public object GetMyFiles([FromQuery] long? lastId, [FromQuery] int size = 10)
    {
        Expression<Func<SysFile, bool>> whereExpression = m => m.CreateUserId == _userManager.UserId;

        whereExpression = whereExpression.AndIf(lastId.HasValue && lastId.Value > 0, m => m.Id < lastId);
        return _sysFileRep.AsQueryable().Where(whereExpression)
                          .OrderByDescending(m => m.Id).Take(size)
                          .Select(m => new { m.Id, m.Url }).ToList();
    }
    public object GetFiles([FromQuery] long? lastId, [FromQuery] int size = 10)
    {
        return _sysFileRep.AsQueryable().WhereIF(lastId.HasValue && lastId.Value > 0, m => m.Id < lastId)
                          .OrderByDescending(m => m.Id).Take(size)
                          .Select(m => new { m.Id, m.Url }).ToList();
    }
}
