using LabPulse.DAL.DataContext.Enums;

namespace LabPulse.BLL.DTOs.Backup;

public class BackupRunbookDto
{
    public string Title { get; set; } = null!;
    public ShellType ShellType { get; set; }
    public string CommandText { get; set; } = null!;
    public string? Notes { get; set; }
}
