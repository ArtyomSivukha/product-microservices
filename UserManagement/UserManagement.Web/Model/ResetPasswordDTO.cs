namespace UserManagement.Web.Model;

public class ResetPasswordDTO
{
    public string PasswordHash {get; set; }
    public string ConfirmPasswordHash  {get; set; }
}