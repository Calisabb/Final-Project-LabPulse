namespace LabPulse.BLL.DTOs.Backup;

public class BackupDto
{
    public int Version { get; set; } = 1;
    public DateTime ExportedAt { get; set; }
    public List<BackupCategoryDto> Categories { get; set; } = new();
    public List<BackupServiceDto> Services { get; set; } = new();
}