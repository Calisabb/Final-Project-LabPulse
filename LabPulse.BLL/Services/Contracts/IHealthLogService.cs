using LabPulse.BLL.DTOs.HealthChecks;

namespace LabPulse.BLL.Services.Contracts;

public interface IHealthLogService
{
    Task<List<HealthLogDto>> GetLastAsync(int serviceId, int count = 50);
    Task<List<HealthLogDto>> GetByPeriodAsync(int serviceId, DateTime from, DateTime to);
}
