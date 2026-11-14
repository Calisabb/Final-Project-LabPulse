using LabPulse.BLL.DTOs.Backup;

namespace LabPulse.BLL.Services.Contracts;

public interface IBackupService
{
    Task<BackupDto> ExportAsync();
    Task<ImportResultDto> ImportAsync(BackupDto backup);
}