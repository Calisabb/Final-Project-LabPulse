using LabPulse.BLL.DTOs.HealthChecks;
using LabPulse.BLL.Mapping;
using LabPulse.BLL.Services.Contracts;
using LabPulse.DAL.Repositories.Contracts;

namespace LabPulse.BLL.Services.Implementations;

internal class HealthLogService : IHealthLogService
{
    private readonly IServiceRepositoryAsync _services;
    private readonly IHealthLogRepositoryAsync _healthLogs;
    private const int MaxLogsPerRequest = 1000;

    public HealthLogService(IServiceRepositoryAsync services, IHealthLogRepositoryAsync healthLogs)
    {
        _services = services;
        _healthLogs = healthLogs;
    }

    public async Task<List<HealthLogDto>> GetByPeriodAsync(int serviceId, DateTime from, DateTime to)
    {
        if (from > to)
            throw new ArgumentException($"From date {from} cannot be greater than to date {to}.");

        if (!await _services.ExistsAsync(s => s.Id == serviceId))
            throw new KeyNotFoundException($"Service with id {serviceId} not found.");

        return (await _healthLogs.GetAllByServiceIdAsync(serviceId, from, to))
            .Select(l=>l.ToDto())
            .ToList();
    }

    public async Task<List<HealthLogDto>> GetLastAsync(int serviceId, int count = 50)
    {
        if (count <= 0)
            throw new ArgumentException($"Count {count} must be greater than zero.");
        if (count > MaxLogsPerRequest)
            throw new ArgumentException($"Count {count} must be less than or equal to {MaxLogsPerRequest}.");

        if (!await _services.ExistsAsync(s => s.Id == serviceId))
            throw new KeyNotFoundException($"Service with id {serviceId} not found.");


        return (await _healthLogs.GetLastAsync(serviceId, count))
            .Select(l => l.ToDto())
            .ToList();
    }
}