using LabPulse.DAL.DataContext.Enums;
using System.ComponentModel.DataAnnotations;

namespace LabPulse.BLL.DTOs.Runbooks;

public class UpdateRunbookDto
{
    [Required, MaxLength(150)] public string Title { get; set; } = null!;
    [EnumDataType(typeof(ShellType))] public ShellType ShellType { get; set; }
    [Required, MaxLength(500)] public string CommandText { get; set; } = null!;
    [MaxLength(500)] public string? Notes { get; set; }
}