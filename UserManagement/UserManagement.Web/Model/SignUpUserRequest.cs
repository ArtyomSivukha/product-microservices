namespace UserManagement.Web.Model;

public class SignUpUserRequest
{
    public string Username {get; set; }
    public string Email {get; set; }
    public string PasswordHash {get; set; }
    public string ConfirmPasswordHash  {get; set; }
    public string ClientUri {get; set; }
}