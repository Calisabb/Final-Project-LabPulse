using LabPulse.DAL.DataContext.Enums;
using System.ComponentModel.DataAnnotations;

namespace LabPulse.BLL.DTOs.Services;

public class UpdateServiceDto
{
    [Required, StringLength(100)] public string Name { get; set; } = null!;
    [Required, StringLength(255)] public string Host { get; set; } = null!;
    [Range(1, 65535)] public int Port { get; set; }
    public bool UseHttps { get; set; }
    public CheckType CheckType { get; set; }
    [Required, StringLength(255)] public string? HealthPath { get; set; }
    [Range(500, 30000)] public int TimeoutMs { get; set; }
    public bool IsMonitoringEnabled { get; set; } = true;
    [StringLength(500)] public string? Description { get; set; }
    [StringLength(50)] public string? Icon { get; set; }
    [Range(1, int.MaxValue)] public int CategoryId { get; set; }
}