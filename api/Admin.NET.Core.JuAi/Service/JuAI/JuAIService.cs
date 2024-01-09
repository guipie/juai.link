

namespace Admin.NET.Core.JuAI;

/// <summary>
/// home主页相关
/// </summary>
[AllowAnonymous]
[ApiDescriptionSettings(ApplicationAppConst.AppApi, Order = 1)]
public class JuAIService : AppBaseService
{
    public readonly SqlSugarRepository<MidjourneyNotify> _midNotifyRepository;

    public readonly SqlSugarRepository<Article> _articleRepository;
    public readonly SqlSugarRepository<ChatGPT> _chatgptRepository;
    public readonly SqlSugarRepository<ChatGPTImage> _chatgptImgRepository;
    public JuAIService(SqlSugarRepository<MidjourneyNotify> midNotifyRepository, SqlSugarRepository<Article> articleRepository,
        SqlSugarRepository<ChatGPT> chatgptRepository, SqlSugarRepository<ChatGPTImage> chatgptImgRepository)
    {
        _midNotifyRepository = midNotifyRepository;
        _articleRepository = articleRepository;
        _chatgptRepository = chatgptRepository;
        _chatgptImgRepository = chatgptImgRepository;
    }

    [DisplayName("广场数据")]
    public async Task<IList<HomeOutput>> GetHome([FromRoute] int page, [FromQuery] int size)
    {
        var images = await _chatgptImgRepository.AsQueryable().OrderByDescending(m => m.Id)
                                        .Select(m => new HomeOutput()
                                        {
                                            Id = m.Id,
                                            Title = m.Prompt,
                                            Text = m.PromptEn,
                                            Image = SqlFunc.IsNullOrEmpty(m.OssUrl) ? m.SourceUrl : m.OssUrl,
                                            HomeType = HomeDataEnum.Drawing.ToString(),
                                        })
                                        .ToPageListAsync(page, size);
        var articles = await _articleRepository.AsQueryable().OrderByDescending(m => m.Id)
                                        .Select(m => new HomeOutput()
                                            {
                                                Id = m.Id,
                                                Title = m.Title,
                                                Text = m.Text,
                                                Image = m.Cover,
                                                HomeType = HomeDataEnum.Article.ToString(),
                                            })
                                         .ToPageListAsync(page, size);

        var chats = await _chatgptRepository.AsQueryable().OrderByDescending(m => m.Id)
                                        .Select(m => new HomeOutput()
                                            {
                                                Id = m.Id,
                                                Title = m.Question,
                                                Text = m.Answer,
                                                HomeType = HomeDataEnum.Chat.ToString(),
                                            })
                                        .ToPageListAsync(page, size);

        return images.Concat(articles).Concat(chats).OrderBy(m => m.Id).ToList();
    }
}
