using LabPulse.DAL.DataContext.Enums;
using System.ComponentModel.DataAnnotations;

namespace LabPulse.BLL.DTOs.Runbooks;

public class CreateRunbookDto
{
    [Required, MaxLength(150)] public string Title { get; set; } = null!;
    [Range(1, int.MaxValue)] public int ServiceId { get; set; }
    [EnumDataType(typeof(ShellType))] public ShellType ShellType { get; set; }
    [Required, MaxLength(500)] public string CommandText { get; set; } = null!;
    [MaxLength(500)] public string? Notes { get; set; }
}