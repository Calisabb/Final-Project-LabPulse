namespace LabPulse.BLL.DTOs.HealthChecks;

public class BulkCheckResultDto
{
    public int Total { get; set; }
    public int Online { get; set; }
    public int Offline { get; set; }
    public DateTime CheckedAt { get; set; }
    public List<ServiceCheckResultDto> Results { get; set; } = new();
}
