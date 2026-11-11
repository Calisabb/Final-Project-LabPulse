using LabPulse.BLL.DTOs.HealthChecks;
using LabPulse.BLL.Mapping;
using LabPulse.BLL.Services.Contracts;
using LabPulse.DAL.DataContext.Entities;
using LabPulse.DAL.DataContext.Enums;
using LabPulse.DAL.Repositories.Contracts;

namespace LabPulse.BLL.Services.Implementations;

internal class HealthCheckService : IHealthCheckService
{
    private const int FailureThreshold = 3;
    private const int MaxParallelProbes = 10;

    private readonly IServiceRepositoryAsync _services;
    private readonly INetworkProbeService _probe;
    private readonly IHealthLogRepositoryAsync _logs;
    private readonly IIncidentRepositoryAsync _incidents;

    public HealthCheckService(
        IServiceRepositoryAsync services,
        INetworkProbeService probe,
        IHealthLogRepositoryAsync logs,
        IIncidentRepositoryAsync incidents)
    {
        _services = services;
        _probe = probe;
        _logs = logs;
        _incidents = incidents;
    }
    private async Task ApplyResultAsync(Service service, ProbeResultDto probe, DateTime now)
    {
        await _logs.AddAsync(probe.ToHealthLog(service.Id, now));

        service.ServiceStatus = probe.IsOnline ? ServiceStatus.Online : ServiceStatus.Offline;
        service.LastCheckedAt = now;
        service.LastResponseTimeMs = probe.ResponseTimeMs;

        var openIncident = await _incidents.GetOpenByServiceAsync(service.Id);
        if (probe.IsOnline)
        { 
            if (openIncident != null)
            {
                openIncident.ResolvedAt = now;
            }
            return;
        }
        if (openIncident != null)
            return;

        var previous = await _logs.GetLastAsync(service.Id, FailureThreshold - 1);
        var enoughFailures = previous.Count == FailureThreshold - 1
                             && previous.All(l => !l.IsOnline);
        if (!enoughFailures)
            return;

        await _incidents.AddAsync(new Incident
        {
            ServiceId = service.Id,
            StartedAt = previous.Min(l => l.CheckedAt),
            Reason = probe.ErrorMessage ?? "Service is not responding"
        });
    }
    public async Task<BulkCheckResultDto> CheckAllAsync(CancellationToken ct = default)
    {
        var services = await _services.GetMonitoredAsync();
        using var semaphore = new SemaphoreSlim(MaxParallelProbes);
        var tasks = services.Select(async service =>
        {
            await semaphore.WaitAsync(ct);
            try
            {
                var probe = await _probe.ProbeAsync(service, ct);
                return (Service: service, Probe: probe);
            }
            finally
            {
                semaphore.Release();
            }
        });
        var probed = await Task.WhenAll(tasks);
        var now = DateTime.UtcNow;
        var results = new List<ServiceCheckResultDto>();
        foreach (var (service, probe) in probed)
        {
            await ApplyResultAsync(service, probe, now);
            results.Add(ToCheckResult(service, probe));
        }
        await _services.SaveChangesAsync();

        return new BulkCheckResultDto
        {
            Total = results.Count,
            Online = results.Count(r => r.IsOnline),
            Offline = results.Count(r => !r.IsOnline),
            CheckedAt = now,
            Results = results
        };
    }
    private static ServiceCheckResultDto ToCheckResult(Service service, ProbeResultDto probe) => new()
    {
        ServiceId = service.Id,
        ServiceName = service.Name,
        IsOnline = probe.IsOnline,
        ResponseTimeMs = probe.ResponseTimeMs,
        StatusCode = probe.StatusCode,
        ErrorMessage = probe.ErrorMessage
    };

    public async Task<ServiceCheckResultDto> CheckServiceAsync(int serviceId, CancellationToken ct = default)
    {
        var service = await _services.GetByIdAsync(serviceId);
        if (service == null)
            throw new KeyNotFoundException($"Service with ID {serviceId} not found.");

        var probe = await _probe.ProbeAsync(service, ct);
        await ApplyResultAsync(service, probe, DateTime.UtcNow);

        await _services.SaveChangesAsync();

        return new ServiceCheckResultDto
        {
            ServiceId = service.Id,
            ServiceName = service.Name,
            IsOnline = probe.IsOnline,
            ResponseTimeMs = probe.ResponseTimeMs,
            StatusCode = probe.StatusCode,
            ErrorMessage = probe.ErrorMessage
        };
    }
}