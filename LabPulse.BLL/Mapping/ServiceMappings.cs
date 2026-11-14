using LabPulse.BLL.DTOs.Services;
using LabPulse.DAL.DataContext.Entities;

namespace LabPulse.BLL.Mapping;

internal static class ServiceMappings
{
    public static ServiceDto ToDto(this Service service)
    {
        return new ServiceDto
        {
            Id = service.Id,
            Name = service.Name,
            Host = service.Host,
            Port = service.Port,
            UseHttps = service.UseHttps,
            CheckType = service.CheckType,
            HealthPath = service.HealthPath,
            TimeoutMs = service.TimeoutMs,
            IsMonitoringEnabled = service.IsMonitoringEnabled,
            Description = service.Description,
            Icon = service.Icon,
            ServiceStatus = service.ServiceStatus,
            LastCheckedAt = service.LastCheckedAt,
            LastResponceTimeMs = service.LastResponseTimeMs,
            CategoryId = service.CategoryId,
            CategoryName = service.Category.Name,
            CategoryColor = service.Category.Color
        };
    }

    public static Service ToEntity(this CreateServiceDto dto)
    {
        return new Service
        {
            Name = dto.Name.Trim(),
            Host = dto.Host.Trim(),
            Port = dto.Port,
            UseHttps = dto.UseHttps,
            CheckType = dto.CheckType,
            HealthPath = dto.HealthPath,
            TimeoutMs = dto.TimeoutMs,
            IsMonitoringEnabled = dto.IsMonitoringEnabled,
            Description = dto.Description,
            Icon = dto.Icon,
            CategoryId = dto.CategoryId
        };
    }

    public static void ApplyTo(this UpdateServiceDto dto, Service service)
    {
        service.Name = dto.Name.Trim();
        service.Host = dto.Host.Trim();
        service.Port = dto.Port;
        service.UseHttps = dto.UseHttps;
        service.CheckType = dto.CheckType;
        service.HealthPath = dto.HealthPath;
        service.TimeoutMs = dto.TimeoutMs;
        service.IsMonitoringEnabled = dto.IsMonitoringEnabled;
        service.Description = dto.Description;
        service.Icon = dto.Icon;
        service.CategoryId = dto.CategoryId;
    }
}