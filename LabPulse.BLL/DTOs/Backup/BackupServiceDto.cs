using LabPulse.DAL.DataContext.Enums;

namespace LabPulse.BLL.DTOs.Backup;

public class BackupServiceDto
{
    public string Name { get; set; } = null!;
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public CheckType CheckType { get; set; }
    public bool UseHttps { get; set; }
    public string? HealthPath { get; set; }
    public int TimeoutMs { get; set; }
    public bool IsMonitoringEnabled { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string CategoryName { get; set; } = null!;
    public List<BackupRunbookDto> Runbooks { get; set; } = new();
}
