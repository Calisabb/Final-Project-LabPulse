namespace LabPulse.BLL.DTOs.HealthChecks;

public class ServiceCheckResultDto
{
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = null!;
    public bool IsOnline { get; set; }
    public int? ResponseTimeMs { get; set; }
    public int? StatusCode { get; set; }
    public string? ErrorMessage { get; set; }
}