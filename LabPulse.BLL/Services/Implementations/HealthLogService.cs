using LabPulse.BLL.DTOs.HealthChecks;
using LabPulse.BLL.Mapping;
using LabPulse.BLL.Services.Contracts;
using LabPulse.DAL.Repositories.Contracts;
using LabPulse.DAL.Repositories.Models;

namespace LabPulse.BLL.Services.Implementations;

internal class HealthLogService : IHealthLogService
{
    private readonly int MaxUptimeDays = 90;
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

    public async Task<List<UptimeDayDto>> GetUptimeAsync(int serviceId, int days = 30)
    {
        if (days <= 0)
            throw new ArgumentException($"Days {days} must be greater than zero.");
        if (days > MaxUptimeDays)
            throw new ArgumentException($"Days {days} must be less than or equal to {MaxUptimeDays}.");

        if (!await _services.ExistsAsync(s => s.Id == serviceId))
            throw new KeyNotFoundException($"Service with id {serviceId} not found.");

        var from = DateTime.UtcNow.Date.AddDays(-(days-1));
        var dailyStats = await _healthLogs.GetDailyStatsAsync(serviceId, from);
        var dict = dailyStats.ToDictionary(s => s.Date, s => s);

        for (int i = 0; i < days; i++)
        {
            var date = from.AddDays(i);
            if (!dict.ContainsKey(date))
            {
                dict[date] = new DailyHealthStat(date, 0, 0);
            }
        }
        return dict.Values
            .OrderBy(s => s.Date)
            .Select(s => new UptimeDayDto
            {
                Date = DateOnly.FromDateTime(s.Date),
                TotalChecks = s.Total,
                OnlineChecks = s.Online,
                UptimePercentage = s.Total > 0 ? (double)s.Online / s.Total * 100 : 0
            })
            .ToList();
    }
}