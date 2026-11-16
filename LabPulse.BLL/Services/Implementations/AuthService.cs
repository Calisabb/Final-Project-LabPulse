using LabPulse.BLL.DTOs.Auth;
using LabPulse.BLL.Services.Contracts;
using LabPulse.DAL.DataContext.Entities;
using Microsoft.AspNetCore.Identity;

namespace LabPulse.BLL.Services.Implementations;

internal class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(UserManager<User> userManager, IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResultDto> LoginAsync(LoginDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var user = await _userManager.FindByNameAsync(dto.UserName);
        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var (token, expiresAt) = _jwtTokenService.GenerateToken(user, roles);

        return new LoginResultDto(token, expiresAt, user.UserName ?? dto.UserName);
    }
}
