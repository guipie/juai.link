namespace Admin.NET.Core.JuAI;

/// <summary>
/// chat模型表种子数据
/// </summary>
public class ChatgptSeedData : ISqlSugarEntitySeedData<ChatModel>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ChatModel> HasData()
    {
        return new List<ChatModel>();
        //return new[]
        //{
        //    //gpt4
        //    new ChatModel{ Id=1, Name="gpt-4", ModelId="gpt-4",ModelType=ChatGPTModelEnum.Chat,Url="/v1/chat/completions",MaxToken=8192,Desc="比任何 GPT-3.5 模型都更强大，能够执行更复杂的任务，并针对聊天进行了优化。将使用我们最新的模型迭代进行更新。", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), TenantId= long.Parse(SqlSugarConst.MainConfigId) },
        //    new ChatModel{ Id=2, Name="gpt-4-0613", ModelId="gpt-4-0613",ModelType=ChatGPTModelEnum.Chat,Url="/v1/chat/completions",MaxToken=8192,Desc="2023 gpt-4 年 6 月 13 日的快照，包含函数调用数据。与此不同 gpt-4 ，此模型将不会收到更新，并将在新版本发布后 3 个月弃用。", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), TenantId= long.Parse(SqlSugarConst.MainConfigId) },
        //    new ChatModel{ Id=3, Name="gpt-4-32k", ModelId="gpt-4-32k",ModelType=ChatGPTModelEnum.Chat,Url="/v1/chat/completions",MaxToken=32768,Desc="与基本gpt-4模式相同的功能，但上下文长度是其 4 倍。将使用我们最新的模型迭代进行更新。", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), TenantId= long.Parse(SqlSugarConst.MainConfigId) },
        //    new ChatModel{ Id=4, Name="gpt-4-32k-0613", ModelId="gpt-4-32k-0613",ModelType=ChatGPTModelEnum.Chat,Url="/v1/chat/completions",MaxToken=32768,Desc="2023 gpt-4-32 年 6 月 13 日的快照。与此不同 gpt-4-32k ，此模型将不会收到更新，并将在新版本发布后 3 个月弃用。", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), TenantId= long.Parse(SqlSugarConst.MainConfigId) },


        //    //gpt3.5
        //    new ChatModel{ Id=5, Name="gpt-3.5-turbo", ModelId="gpt-3.5-turbo",ModelType=ChatGPTModelEnum.Chat,Url="/v1/chat/completions",MaxToken=4096,Desc="功能最强大的 GPT-3.5 型号，针对聊天进行了优化，成本仅为 text-davinci-003 的 1/10。将使用我们最新的模型迭代进行更新。", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), TenantId= long.Parse(SqlSugarConst.MainConfigId) },
        //    new ChatModel{ Id=6, Name="gpt-3.5-turbo-16k", ModelId="gpt-3.5-turbo-16k",ModelType=ChatGPTModelEnum.Chat,Url="/v1/chat/completions",MaxToken=16384,Desc="与标准模型gpt-3.5-turbo有相同的功能，但该模型是其 4 倍的上下文。", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), TenantId= long.Parse(SqlSugarConst.MainConfigId) },
        //    new ChatModel{ Id=7, Name="gpt-3.5-turbo-0613", ModelId="gpt-3.5-turbo-0613",ModelType=ChatGPTModelEnum.Chat,Url="/v1/chat/completions",MaxToken=4096 ,Desc="2023 gpt-3.5-turbo 年 6 月 13 日的快照，包含函数调用数据。与此不同 gpt-3.5-turbo ，此模型将不会收到更新，并将在新版本发布后 3 个月弃用。", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), TenantId= long.Parse(SqlSugarConst.MainConfigId) },
        //    new ChatModel{ Id=8, Name="gpt-3.5-turbo-16k-0613", ModelId="gpt-3.5-turbo-16k-0613",ModelType=ChatGPTModelEnum.Chat,Url="/v1/chat/completions",MaxToken=16384,Desc="2023 gpt-3.5-turbo-16k 年 6 月 13 日的快照。与此不同 gpt-3.5-turbo-16k ，此模型将不会收到更新，并将在新版本发布后 3 个月弃用。", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), TenantId= long.Parse(SqlSugarConst.MainConfigId) },
        //};
    }
}