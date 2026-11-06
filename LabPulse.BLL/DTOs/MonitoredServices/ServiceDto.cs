using LabPulse.DAL.DataContext.Enums;

namespace LabPulse.BLL.DTOs.Services;

public class ServiceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public bool UseHttps { get; set; }
    public CheckType CheckType { get; set; }
    public string? HealthPath { get; set; }
    public int TimeoutMs { get; set; }
    public bool IsMonitoringEnabled { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public ServiceStatus ServiceStatus { get; set; }
    public DateTime? LastCheckedAt { get; set; }
    public int? LastResponceTimeMs { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? CategoryColor { get; set; }
}