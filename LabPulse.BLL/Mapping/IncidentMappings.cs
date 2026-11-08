using LabPulse.BLL.DTOs.Incidents;
using LabPulse.DAL.DataContext.Entities;

namespace LabPulse.BLL.Mapping;

internal static class IncidentMappings
{
    public static IncidentDto ToDto(this Incident i)
    {
        return new IncidentDto
        {
            Id = i.Id,
            ServiceId = i.ServiceId,
            ServiceName = i.Service.Name,
            StartedAt = i.StartedAt,
            ResolvedAt = i.ResolvedAt,
            IsOpen = i.ResolvedAt == null,
            DurationSeconds = (long)((i.ResolvedAt ?? DateTime.UtcNow) - i.StartedAt).TotalSeconds,
            Reason = i.Reason
        };
    }
}