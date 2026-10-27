namespace LabPulse.DAL.DataContext.Entities;

public class HealthLog
{
    public long Id { get; set; }
    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;
    public bool IsOnline { get; set; }
    public int? ResponceTimeMs { get; set; }
    public int? StatusCode { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? CheckedAt { get; set; }
}
