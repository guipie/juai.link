


namespace Admin.NET.Core.JuAI.Service.AI;

public class ChatResult
{
    public int Code { get; set; }
    public string Message { get; set; }
    public ChatResult(string msg, int code = 200)
    {
        this.Message = msg;
        this.Code = code;
    }
}
