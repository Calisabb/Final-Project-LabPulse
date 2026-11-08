namespace LabPulse.BLL.DTOs.HealthChecks;

public class ProbeResultDto
{
    public bool IsOnline { get; set; }
    public int? ResponseTimeMs { get; set; }
    public int? StatusCode { get; set; }
    public string? ErrorMessage { get; set; }
}