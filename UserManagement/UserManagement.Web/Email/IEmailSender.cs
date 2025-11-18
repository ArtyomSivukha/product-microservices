namespace UserManagement.Web.Email;

public interface IEmailSender
{
    Task SendEmailAsync(Message message);
}