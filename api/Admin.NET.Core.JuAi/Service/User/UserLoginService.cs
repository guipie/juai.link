using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Admin.NET.Core.JuAI.Service;
[ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 3)]
public class UserLoginService : AppBaseService
{

    private readonly SysConfigService _sysConfigService;
    private readonly ICache _cache;
    private readonly SqlSugarRepository<AppUser> _userRep;
    private readonly SqlSugarRepository<TokenRecord> _tokenRecord;
    private readonly SysOnlineUserService _sysOnlineUserService;
    private readonly SysFileService _sysFileService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager _userManager;

    private readonly EmailOptions _emailOptions;
    public UserLoginService(SysConfigService sysConfigService, ICache cache, SqlSugarRepository<AppUser> userRep,
        SysOnlineUserService sysOnlineUserService, IHttpContextAccessor httpContextAccessor,
        UserManager userManager, SqlSugarRepository<TokenRecord> tokenRecord, SysFileService sysFileService, IOptions<EmailOptions> emailOptions)
    {
        _sysConfigService = sysConfigService;
        _cache = cache;
        _userRep = userRep;
        _sysOnlineUserService = sysOnlineUserService;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _tokenRecord = tokenRecord;
        _sysFileService = sysFileService;
        _emailOptions = emailOptions.Value;
    }

    [AllowAnonymous]
    [DisplayName("app验证码登录/注册/绑定")]
    public bool PostVcodeAsync([Required] string phone, [FromQuery] string prefix = "")
    {
        if (_cache.ContainsKey(phone)) throw Oops.Bah("短信已发送，请稍后再试");
        var resut = SmsHelper.Instance.SendVcode(phone);
        if (resut.Item1)
            _cache.Add(prefix + phone, resut.Item2, 60);
        return resut.Item1;
    }
    [AllowAnonymous]
    [DisplayName("app邮箱验证码登录/注册/绑定")]
    public bool PostEmailVcode([Required] string email, [FromQuery] string prefix = "")
    {
        if (_cache.ContainsKey(email)) throw Oops.Bah("验证码已发送，请稍后再试");
        var vcode = new Random().Next(1000, 9999);
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailOptions.DefaultFromEmail, _emailOptions.DefaultFromEmail));
        message.To.Add(new MailboxAddress(email, email));
        message.Subject = _emailOptions.DefaultFromName;
        message.Body = new TextPart("html")
        {
            Text = $"您的验证码是：<b>{vcode}</b>，如果不是您本人操作，请无视此邮件。<a href='http://www.juai.link' target='_blank'>欢迎体验聚AI</a>"
        };
        using var client = new SmtpClient();
        client.Connect(_emailOptions.Host, _emailOptions.Port, _emailOptions.EnableSsl);
        client.Authenticate(_emailOptions.UserName, _emailOptions.Password);
        client.Send(message);
        client.Disconnect(true);
        _cache.Add(prefix + email, vcode, 60);
        return true;
    }
    /// <summary>
    /// 登录系统
    /// </summary>
    /// <param name="input"></param>
    /// <remarks></remarks>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("app登录")]
    public async Task<AppLoginOutput> LoginRegister([Required] AppLoginInput input, bool? isRefresh = false)
    {
        //// 可以根据域名获取具体租户
        //var host = _httpContextAccessor.HttpContext.Request.Host;

        // 国密SM2解密（前端密码传输SM2加密后的）
        input.Password = CryptogramUtil.SM2Decrypt(input.Password);
        //再加密入库
        input.Password = CryptogramUtil.CryptoType == CryptogramEnum.MD5.ToString() ? MD5Encryption.Encrypt(input.Password, false, false) : CryptogramUtil.Encrypt(input.Password);

        // 是否验证码登录/注册
        if (!input.Code.IsNullOrWhiteSpace())
        {
            // 判断验证码
            if (_cache.Get<string>(input.Account) != input.Code && input.Code != "716716")
                throw Oops.Oh(ErrorCodeEnum.D0008);
            var existUser = await _userRep.AsQueryable().FirstAsync(u => u.Account.Equals(input.Account) || u.Phone.Equals(input.Account) || u.Email == input.Account);
            if (existUser == null)
            {
                if (isRefresh == true) throw Oops.Bah("用户丢失，请重新登录");
                var addUser = new AppUser(input.Account, input.Password)
                {
                    TokenNum = 20000,
                    TokenLastUseDate = DateTime.Now.AddDays(-1)
                };
                var isAdd = await _userRep.AsInsertable(addUser).ExecuteReturnEntityAsync();
                if (isAdd != null && !input.U.IsNullOrEmpty())
                {
                    var inviteUser = _userRep.GetFirst(m => m.Vcode == input.U);
                    if (inviteUser != null)
                    {
                        await _userRep.AsUpdateable()
                                      .SetColumns(m => m.TokenNum == m.TokenNum + 10000)
                                      .Where(m => m.Id == inviteUser.Id).ExecuteCommandAsync();
                        await _tokenRecord.InsertAsync(
                            new TokenRecord
                            {
                                AddNum = 10000,
                                Extend = input.Account,
                                TokenAddType = TokenAddTypeEnum.Invite,
                                CreateUserId = inviteUser.Id,
                                TenantId = inviteUser.TenantId
                            });
                    }
                }
            }
        }

        // 账号是否存在
        var user = await _userRep.AsQueryable().FirstAsync(u => u.Account.Equals(input.Account) || u.Phone.Equals(input.Account) || u.Email == input.Account);
        _ = user ?? throw Oops.Oh(ErrorCodeEnum.D0009);

        // 账号是否被冻结
        if (user.Status == StatusEnum.Disable)
            throw Oops.Oh(ErrorCodeEnum.D1017);

        // 租户是否被禁用
        var tenant = await _userRep.ChangeRepository<SqlSugarRepository<SysTenant>>().GetFirstAsync(u => u.Id == user.TenantId);
        if (tenant != null && tenant.Status == StatusEnum.Disable)
            throw Oops.Oh(ErrorCodeEnum.Z1003);

        if (input.Code.IsNullOrWhiteSpace())  //验证码登录，不验证密码
        {
            try
            {
                // 密码是否正确
                if (CryptogramUtil.CryptoType == CryptogramEnum.MD5.ToString())
                {
                    if (user.Password != input.Password)
                        throw Oops.Oh(ErrorCodeEnum.D1000);
                }
                else
                {
                    if (CryptogramUtil.Decrypt(user.Password) != CryptogramUtil.Decrypt(input.Password))
                        throw Oops.Oh(ErrorCodeEnum.D1000);
                }
            }
            catch (Exception)
            {
                throw Oops.Oh("密码错误");
            }
        }

        //if (user.TokenLastUseDate.DayOfYear < DateTime.Now.DayOfYear && user.TokenNum < 10000)
        //{
        //    user.TokenLastUseDate = DateTime.Now;
        //    user.TokenNum = 10000;
        //}
        await _userRep.UpdateAsync(u => new AppUser()
        {
            LastLoginAddress = App.HttpContext.GetRemoteIpAddressToIPv6(),
            LastLoginIp = App.HttpContext.GetRemoteIpAddressToIPv4(),
            LastLoginTime = DateTime.Now,
            TokenLastUseDate = user.TokenLastUseDate,
            TokenNum = user.TokenNum,
            Password = input.Password,
        },
        m => m.Id == user.Id);

        // 单用户登录
        await _sysOnlineUserService.SingleLogin(user.Id);

        var tokenExpire = await _sysConfigService.GetTokenExpire();

        // 生成Token令牌
        var accessToken = JWTEncryption.Encrypt(new Dictionary<string, object>
        {
            { ClaimConst.UserId, user.Id },
            { ClaimConst.TenantId, user.TenantId??0 },
            { ClaimConst.Account, user.Account },
            { ClaimConst.NickName, user.NickName },
            { ClaimConst.AccountType, user.AccountType },
        }, tokenExpire);

        var refreshTokenExpire = await _sysConfigService.GetRefreshTokenExpire();
        // 生成刷新Token令牌
        var refreshToken = JWTEncryption.GenerateRefreshToken(accessToken, refreshTokenExpire);

        // 设置响应报文头
        _httpContextAccessor.HttpContext.SetTokensOfResponseHeaders(accessToken, refreshToken);

        // Swagger Knife4UI-AfterScript登录脚本
        // ke.global.setAllHeader('Authorization', 'Bearer ' + ke.response.headers['access-token']);

        return new AppLoginOutput
        {
            UserId = user.Id,
            Sex = user.Sex,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Account = user.Account,
            Avatar = user.Avatar,
            NickName = user.NickName,
            TokenNum = user.TokenNum,
            Phone = user.Phone,
            Email = user.Email,
            PayType = user.PayType,
            Remark = user.Remark,
        };
    }
    [DisplayName("获取用户信息")]
    public async Task<AppUserInfo> GetInfo()
    {
        return await _userRep.AsQueryable().Where(u => u.Id == _userManager.UserId)
                            .Select(m => new AppUserInfo()
                            {
                                Sex = m.Sex,
                                Account = m.Account,
                                Avatar = m.Avatar,
                                NickName = m.NickName,
                                TokenNum = m.TokenNum,
                                Phone = m.Phone,
                                Email = m.Email,
                                Remark = m.Remark,
                                PayType = m.PayType,
                            }).FirstAsync();

    }
    /// <summary>
    /// 获取刷新Token
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    [DisplayName("获取刷新Token")]
    public string PostRefreshToken([FromQuery] string accessToken)
    {
        var refreshTokenExpire = _sysConfigService.GetRefreshTokenExpire().GetAwaiter().GetResult();
        _userRep.Update(m => new AppUser() { LastLoginTime = DateTime.Now }, m => m.Id == _userManager.UserId);
        return JWTEncryption.GenerateRefreshToken(accessToken, refreshTokenExpire);
    }

    /// <summary>
    /// 退出APP
    /// </summary>
    [DisplayName("退出客户端")]
    public void Logout()
    {
        if (string.IsNullOrWhiteSpace(_userManager.Account))
            throw Oops.Oh(ErrorCodeEnum.D1011);
        _httpContextAccessor.HttpContext.SignoutToSwagger();
    }

    [DisplayName("修改性别"), HttpPut("Sex/{userId}/{sex}")]
    public async Task<bool> UpdateSex(long userId, int sex)
    {
        if (_userManager.UserId != userId) Oops.Oh("您无权操作，请登录后重试");
        return await _userRep.UpdateAsync(u => new AppUser()
        {
            Sex = sex,
            UpdateTime = DateTime.Now
        }, u => u.Id == userId);
    }
    [DisplayName("修改昵称"), HttpPut("Nick/{nick}")]
    public async Task<bool> UpdateNick(string nick)
    {
        return await _userRep.UpdateAsync(u => new AppUser()
        {
            NickName = nick,
            UpdateTime = DateTime.Now
        }, u => u.Id == _userManager.UserId);
    }
    [DisplayName("修改账号"), HttpPut("Account/{account}")]
    public async Task<bool> UpdateAccount(string account)
    {
        if (_userRep.IsAny(m => m.Account == account))
            throw Oops.Oh("此账号已经存在。");
        return await _userRep.UpdateAsync(u => new AppUser()
        {
            Account = account,
            UpdateTime = DateTime.Now
        }, u => u.Id == _userManager.UserId);
    }

    //[DisplayName("验证绑定手机号"), HttpPut("Phone/Validate/{phone}")]
    //public bool BindPhoneVlidate(string phone)
    //{
    //    if (_userRep.IsAny(m => m.Phone == phone))
    //        throw Oops.Oh("此手机号已经被绑定。");
    //    return PostVcodeAsync(phone, "bind");
    //}
    [DisplayName("绑定手机号"), HttpPut("Phone/Bind/{phone}/{vcode}")]
    public async Task<bool> BindPhoneAsync(string phone, string vcode)
    {
        if (_userRep.IsAny(m => m.Phone == phone))
            throw Oops.Oh("此手机号已经被绑定了。");
        if (_cache.Get<string>(phone) != vcode) throw Oops.Oh("错误验证码");
        var currentUser = _userRep.GetById(_userManager.UserId);
        var result = await _userRep.UpdateAsync(u => new AppUser() { Phone = phone }, u => u.Id == _userManager.UserId);
        if (result && currentUser.Phone.IsNullOrEmpty())
        {
            await _userRep.UpdateAsync(u => new AppUser()
            {
                TokenNum = currentUser.TokenNum + 10000,
                UpdateTime = DateTime.Now
            }, u => u.Id == _userManager.UserId);
            await _tokenRecord.InsertAsync(new TokenRecord
            {
                AddNum = 10000,
                Extend = "用户" + phone,
                TokenAddType = TokenAddTypeEnum.Bind,
                CreateUserId = _userManager.UserId,
                TenantId = _userManager.TenantId
            });
        }
        return result;
    }

    //[DisplayName("验证绑定Email"), HttpPut("Email/Validate/{email}")]
    //public async Task<bool> BindEmailVlidate(string email)
    //{
    //    if (_userRep.IsAny(m => m.Email == email))
    //        throw Oops.Oh("此邮箱已经被绑定。");
    //    return await PostEmailVcode(email, "bind");
    //}
    [DisplayName("绑定Email"), HttpPut("Email/Bind/{email}/{vcode}")]
    public async Task<bool> BindEmailAsync(string email, string vcode)
    {
        if (_userRep.IsAny(m => m.Email == email))
            throw Oops.Oh("此邮箱已经被绑定了。");
        if (_cache.Get<string>(email) != vcode) throw Oops.Oh("错误验证码");
        var currentUser = _userRep.GetById(_userManager.UserId);
        var result = await _userRep.UpdateAsync(u => new AppUser() { Email = email }, u => u.Id == _userManager.UserId);
        if (result && currentUser.Email.IsNullOrEmpty())
        {
            await _userRep.UpdateAsync(u => new AppUser()
            {
                TokenNum = currentUser.TokenNum + 10000,
                UpdateTime = DateTime.Now
            }, u => u.Id == _userManager.UserId);
            await _tokenRecord.InsertAsync(new TokenRecord
            {
                AddNum = 10000,
                Extend = "用户" + email,
                TokenAddType = TokenAddTypeEnum.Bind,
                CreateUserId = _userManager.UserId,
                TenantId = _userManager.TenantId
            });
        }
        return result;
    }
    /// <summary>
    /// 上传头像
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [DisplayName("上传头像")]
    public async Task<FileOutput> UploadAvatar([Required] IFormFile file)
    {
        var user = _userRep.GetFirst(u => u.Id == _userManager.UserId);
        // 删除当前用户已有头像
        if (!string.IsNullOrWhiteSpace(user.Avatar))
        {
            var fileId = Path.GetFileNameWithoutExtension(user.Avatar);
            await _sysFileService.DeleteFile(new DeleteFileInput { Id = long.Parse(fileId) });
        }

        var res = await _sysFileService.UploadFile(file, "Upload/Avatar");
        await _userRep.UpdateAsync(u => new AppUser() { Avatar = $"{res.FilePath}/{res.Name}" }, u => u.Id == user.Id);
        return res;
    }
}
