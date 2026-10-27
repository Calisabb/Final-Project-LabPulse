using LabPulse.DAL.DataContext.Enums;

namespace LabPulse.DAL.DataContext.Entities;
public class Runbook : BaseEntity
{
    public string Title { get; set; } = null!;
    public Service Service { get; set; } = null!;
    public int ServiceId { get; set; }
    public ShellType ShellType { get; set; }
    public string CommandText { get; set; } = null!;
    public string? Notes { get; set; } = null!;
}
