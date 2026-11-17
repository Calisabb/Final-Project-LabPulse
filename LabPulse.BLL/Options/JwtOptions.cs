using System.ComponentModel.DataAnnotations;

namespace LabPulse.BLL.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required(ErrorMessage = "Issuer is required.")]
    public string Issuer { get; set; } = string.Empty;

    [Required(ErrorMessage = "Audience is required.")]
    public string Audience { get; set; } = string.Empty;

    [Required(ErrorMessage = "Key is required.")]
    [MinLength(32, ErrorMessage = "Key must be at least 32 characters.")]
    public string Key { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "ExpiresMinutes must be greater than 0.")]
    public int ExpiresMinutes { get; set; }
}
