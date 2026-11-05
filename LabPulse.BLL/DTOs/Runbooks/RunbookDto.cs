using LabPulse.DAL.DataContext.Enums;

namespace LabPulse.BLL.DTOs.Runbooks;

public class RunbookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public int ServiceId { get; set; }
    public ShellType ShellType { get; set; }
    public string CommandText { get; set; } = null!;
    public string? Notes { get; set; } = null!;
}
