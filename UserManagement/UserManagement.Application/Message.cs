namespace UserManagement.Application;

public class Message
{
    public IEnumerable<string> To { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
    public Message(IEnumerable<string> to, string subject, string content)
    {
        To = to.ToArray();
        Subject = subject;
        Content = content;        
    }
}