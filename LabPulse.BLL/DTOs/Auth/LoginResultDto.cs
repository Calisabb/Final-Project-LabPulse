namespace LabPulse.BLL.DTOs.Auth;

public class LoginResultDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string UserName { get; set; } = string.Empty;

    public LoginResultDto() { }

    public LoginResultDto(string token, DateTime expiresAt, string userName)
    {
        Token = token;
        ExpiresAt = expiresAt;
        UserName = userName;
    }
}
