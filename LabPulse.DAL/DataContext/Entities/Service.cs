using LabPulse.DAL.DataContext.Enums;

namespace LabPulse.DAL.DataContext.Entities;
public class Service : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public CheckType CheckType { get; set; }
    public string? HealthPath { get; set; }
    public int TimeoutMs { get; set; } = 3000;
    public bool IsMonitoringEnabled { get; set; } = true;
    public ServiceStatus ServiceStatus { get; set; }
    public DateTime? LastCheked { get; set; }
    public int? LastResponceTimeMs { get; set; }

    public Category Category { get; set; } = null!;
    public int CategoryId { get; set; }
    public ICollection<HealthLog> HealthLogs { get; set; } = new List<HealthLog>();
    public ICollection<Incident> Incidents { get; set; } = new List<Incident>();
    public ICollection<Runbook> Runbooks { get; set;} = new List<Runbook>();
}
