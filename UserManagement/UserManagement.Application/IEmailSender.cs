namespace UserManagement.Application;

public interface IEmailSender
{
    Task SendEmailAsync(Message message);
}