using LabPulse.BLL.DTOs.Runbooks;
using LabPulse.DAL.DataContext.Entities;

namespace LabPulse.BLL.Mapping;

internal static class RunbookMappings
{
    public static RunbookDto ToDto(this Runbook runbook)
    {
        return new RunbookDto
        {
            Id = runbook.Id,
            Title = runbook.Title,
            ServiceId = runbook.ServiceId,
            ShellType = runbook.ShellType,
            CommandText = runbook.CommandText,
            Notes = runbook.Notes
        };
    }
    public static Runbook ToEntity(this CreateRunbookDto dto)
    {
        return new Runbook
        {
            Title = dto.Title.Trim(),
            ServiceId = dto.ServiceId,
            ShellType = dto.ShellType,
            CommandText = dto.CommandText.Trim(),
            Notes = dto.Notes?.Trim()
        };
    }
    public static void ApplyTo(this UpdateRunbookDto dto, Runbook runbook)
    {
        runbook.Title = dto.Title.Trim();
        runbook.ShellType = dto.ShellType;
        runbook.CommandText = dto.CommandText.Trim();
        runbook.Notes = dto.Notes?.Trim();
    }
}
