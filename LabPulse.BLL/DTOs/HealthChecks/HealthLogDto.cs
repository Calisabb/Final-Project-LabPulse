namespace LabPulse.BLL.DTOs.HealthChecks;

public class HealthLogDto
{
    public long Id { get; set; }
    public int ServiceId { get; set; }
    public bool IsOnline { get; set; }
    public int? ResponseTimeMs { get; set; }
    public int? StatusCode { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CheckedAt { get; set; }
}