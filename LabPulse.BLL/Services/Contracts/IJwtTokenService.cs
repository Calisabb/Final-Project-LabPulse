using LabPulse.DAL.DataContext.Entities;

namespace LabPulse.BLL.Services.Contracts;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user, IEnumerable<string> roles);
}
