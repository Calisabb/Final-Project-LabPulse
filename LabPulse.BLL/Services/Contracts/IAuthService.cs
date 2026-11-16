using LabPulse.BLL.DTOs.Auth;

namespace LabPulse.BLL.Services.Contracts;

public interface IAuthService
{
    Task<LoginResultDto> LoginAsync(LoginDto dto);
}
