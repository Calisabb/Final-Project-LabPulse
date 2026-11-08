using LabPulse.BLL.DTOs.HealthChecks;
using LabPulse.DAL.DataContext.Entities;

namespace LabPulse.BLL.Mapping;

internal static class HealthLogMappings
{
    public static HealthLogDto ToDto(this HealthLog log) => new()
    {
        Id = log.Id,
        ServiceId = log.ServiceId,
        IsOnline = log.IsOnline,
        ResponseTimeMs = log.ResponseTimeMs,
        StatusCode = log.StatusCode,
        ErrorMessage = log.ErrorMessage,
        CheckedAt = log.CheckedAt
    };

    public static HealthLog ToHealthLog(
        this ProbeResultDto result, int serviceId, DateTime checkedAt) => new()
        {
            ServiceId = serviceId,
            IsOnline = result.IsOnline,
            ResponseTimeMs = result.ResponseTimeMs,
            StatusCode = result.StatusCode,
            ErrorMessage = Truncate(result.ErrorMessage, 500),
            CheckedAt = checkedAt
        };
    private static string? Truncate(string? text, int max)
    { 
        return text is null || text.Length <= max ? text : text[..max];
    }     
}