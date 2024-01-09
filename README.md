<div align="center"><h1>juai.link</h1></div>
<div align="center"><h3>聚AI，AI集成的开源公共社区 </h3></div>

<!--
<div align="center">
[![star](https://gitee.com/zuohuaijun/Admin.NET/badge/star.svg?theme=dark)](https://gitee.com/zuohuaijun/Admin.NET/stargazers)
[![fork](https://gitee.com/zuohuaijun/Admin.NET/badge/fork.svg?theme=dark)](https://gitee.com/zuohuaijun/Admin.NET/members)
[![GitHub license](https://img.shields.io/badge/license-MIT-yellow)](https://gitee.com/zuohuaijun/Admin.NET/blob/next/LICENSE)

</div>
-->

## 🎁框架介绍
后端NET8+前端Nuxt3开发，基于 【[Admin.NET](https://gitee.com/zuohuaijun/Admin.NET)】 

## 🍁说明
1.  支持各种数据库，后台配置文件自行修改（自动生成数据库及种子数据）
2.  后端、前端运行步骤：安装依赖pnpm install  运行pnpm run dev  打包pnpm run build
3.  微信号：15100305 

## 🍎效果截图
<table>
    <tr>
        <td><img src="https://gitee.com/zuohuaijun/Admin.NET/raw/next/doc/img/1.png"/></td>
        <td><img src="https://gitee.com/zuohuaijun/Admin.NET/raw/next/doc/img/2.png"/></td>
        <td><img src="https://gitee.com/zuohuaijun/Admin.NET/raw/next/doc/img/3.png"/></td>
        <td><img src="https://gitee.com/zuohuaijun/Admin.NET/raw/next/doc/img/4.png"/></td>
    </tr>
    <tr>
        <td><img src="https://gitee.com/zuohuaijun/Admin.NET/raw/next/doc/img/5.png"/></td>
        <td><img src="https://gitee.com/zuohuaijun/Admin.NET/raw/next/doc/img/6.png"/></td>
        <td><img src="https://gitee.com/zuohuaijun/Admin.NET/raw/next/doc/img/7.png"/></td>
        <td><img src="https://gitee.com/zuohuaijun/Admin.NET/raw/next/doc/img/8.png"/></td>
    </tr>
    <tr>
        <td><img src="https://gitee.com/zuohuaijun/Admin.NET/raw/next/doc/img/9.png"/></td>
        <td><img src="https://gitee.com/zuohuaijun/Admin.NET/raw/next/doc/img/10.png"/></td>
        <td><img src="https://gitee.com/zuohuaijun/Admin.NET/raw/next/doc/img/11.png"/></td>
        <td><img src="https://gitee.com/zuohuaijun/Admin.NET/raw/next/doc/img/12.png"/></td>
    </tr>
    <tr>
        <td><img src="https://gitee.com/zuohuaijun/Admin.NET/raw/next/doc/img/13.png"/></td>
        <td><img src="https://gitee.com/zuohuaijun/Admin.NET/raw/next/doc/img/14.png"/></td>
        <td><img src="https://gitee.com/zuohuaijun/Admin.NET/raw/next/doc/img/15.png"/></td>
        <td><img src="https://gitee.com/zuohuaijun/Admin.NET/raw/next/doc/img/16.png"/></td>
    </tr>
</table>

## 🍖前台内置功能（[聚AI](http://juai.link)）
 1.注册登录(手机号，邮箱)。[✓] 
 2.文章发布。[✓] 
 3.专栏发布。[✓] 
 4.点赞评论分享。 [✓] 
 5.第三方支付功能。 [✓]  
 6.其他.....


## 🍖后台内置功能
 1. 主控面板：控制台页面，可进行工作台，分析页，统计等功能的展示。
 2. 用户管理：对企业用户和系统管理员用户的维护，可绑定用户职务，机构，角色，数据权限等。
 3. 机构管理：公司组织架构维护，支持多层级结构的树形结构。
 4. 职位管理：用户职务管理，职务可作为用户的一个标签。
 5. 菜单管理：配置系统菜单，操作权限，按钮权限标识等，包括目录、菜单、按钮。
 6. 角色管理：角色绑定菜单后，可限制相关角色的人员登录系统的功能范围。角色也可以绑定数据授权范围。
 7. 字典管理：对系统中经常使用的一些较为固定的数据进行维护。
 8. 访问日志：用户的登录和退出日志的查看和管理。
 9. 操作日志：系统正常操作日志记录和查询；系统异常信息日志记录和查询。
10. 服务监控：服务器的运行状态，CPU、内存、网络等信息数据的查看。
11. 在线用户：当前系统在线用户的查看，包括强制下线。基于 SignalR 实现。
12. 公告管理：系统通知公告信息发布维护，使用 SignalR 实现对用户实时通知。
13. 文件管理：文件的上传下载查看等操作，文件可使用本地存储，阿里云oss、腾讯cos等接入，支持拓展。
14. 任务调度：采用 Sundial，.NET 功能齐全的开源分布式作业调度系统。
15. 系统配置：系统运行的参数的维护，参数的配置与系统运行机制息息相关。
16. 邮件短信：发送邮件功能、发送短信功能。
17. 系统接口：使用 Swagger 生成相关 api 接口文档。支持 Knife4jUI 皮肤。
18. 代码生成：可以一键生成前后端代码，自定义配置前端展示控件，让开发更快捷高效。
19. 在线构建器：拖动表单元素生成相应的 VUE 代码(支持vue3)。
20. 对接微信：对接微信小程序开发，包括微信支付。
21. 导入导出：采用 Magicodes.IE 支持文件导入导出，支持根据H5模板生成PDF等报告文件。
22. 限流控制：采用 AspNetCoreRateLimit 组件实现对接口访问限制。
23. ES 日志：通过 NEST 组件实现日志存取到 Elasticsearch 日志系统。
24. 开放授权：支持OAuth 2.0开放标准授权登录，比如微信。
25. APIJSON：适配腾讯APIJSON协议，支持后端0代码，[使用文档](https://github.com/liaozb/APIJSON.NET)。 

## 🎀感谢支持
```
如果对您有帮助，请点击右上角⭐Star关注，感谢支持开源！
``` 

## 💐特别鸣谢
- 👉 vue-next-admin：[https://lyt-top.gitee.io/vue-next-admin-doc-preview/](https://lyt-top.gitee.io/vue-next-admin-doc-preview/)
- 👉 SqlSugar：[https://gitee.com/dotnetchina/SqlSugar](https://gitee.com/dotnetchina/SqlSugar)
- 👉 NewLife.Redis：[https://github.com/NewLifeX/NewLife.Redis](https://github.com/NewLifeX/NewLife.Redis)
- 👉 Magicodes.IE：[https://gitee.com/magicodes/Magicodes.IE](https://gitee.com/magicodes/Magicodes.IE)
- 👉 SKIT.FlurlHttpClient.Wechat：[https://gitee.com/fudiwei/DotNetCore.SKIT.FlurlHttpClient.Wechat](https://gitee.com/fudiwei/DotNetCore.SKIT.FlurlHttpClient.Wechat)
- 👉 IdGenerator：[https://github.com/yitter/idgenerator](https://github.com/yitter/idgenerator)
- 👉 UAParser：[https://github.com/ua-parser/uap-csharp/](https://github.com/ua-parser/uap-csharp/)
- 👉 OnceMi.AspNetCore.OSS：[https://github.com/oncemi/OnceMi.AspNetCore.OSS](https://github.com/oncemi/OnceMi.AspNetCore.OSS)
- 👉 NETCore.MailKit：[https://github.com/myloveCc/NETCore.MailKit](https://github.com/myloveCc/NETCore.MailKit)
- 👉 Lazy.Captcha.Core：[https://gitee.com/pojianbing/lazy-captcha](https://gitee.com/pojianbing/lazy-captcha)
- 👉 AspNetCoreRateLimit：[https://github.com/stefanprodan/AspNetCoreRateLimit](https://github.com/stefanprodan/AspNetCoreRateLimit)
- 👉 Elasticsearch.Net：[https://github.com/elastic/elasticsearch-net](https://github.com/elastic/elasticsearch-net)
- 👉 Masuit.Tools：[https://gitee.com/masuit/Masuit.Tools](https://gitee.com/masuit/Masuit.Tools)
- 👉 IGeekFan.AspNetCore.Knife4jUI：[https://github.com/luoyunchong/IGeekFan.AspNetCore.Knife4jUI](https://github.com/luoyunchong/IGeekFan.AspNetCore.Knife4jUI)
- 👉 AspNet.Security.OAuth.Providers：[https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers](https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers)
- 👉 System.Linq.Dynamic.Core：[https://github.com/zzzprojects/System.Linq.Dynamic.Core](https://github.com/zzzprojects/System.Linq.Dynamic.Core)
- 👉 APIJSON.NET：[https://github.com/liaozb/APIJSON.NET](https://github.com/liaozb/APIJSON.NET)
- 👉 vue-plugin-hiprint：[https://gitee.com/CcSimple/vue-plugin-hiprint](https://gitee.com/CcSimple/vue-plugin-hiprint)
