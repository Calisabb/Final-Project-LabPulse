namespace LabPulse.BLL.DTOs.HealthChecks;

public class UptimeDayDto
{
    public DateOnly Date { get; set; }
    public int TotalChecks { get; set; }
    public int OnlineChecks { get; set; }
    public double UptimePercentage { get; set; }
}
