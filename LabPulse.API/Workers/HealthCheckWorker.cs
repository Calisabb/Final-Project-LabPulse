using LabPulse.BLL.Services.Contracts;

namespace LabPulse.API.Workers;

public class HealthCheckWorker : BackgroundService
{
    private static readonly TimeSpan CleanupEvery = TimeSpan.FromHours(24);
    
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<HealthCheckWorker> _logger;
    private readonly TimeSpan _interval;
    private DateTime _lastCleanup = DateTime.MinValue;

    public HealthCheckWorker(IServiceScopeFactory scopeFactory, ILogger<HealthCheckWorker> logger, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        var seconds = configuration.GetValue("HealthCheck:IntervalSeconds", 30);
        _interval = TimeSpan.FromSeconds(Math.Max(seconds, 5));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Health check worker started, interval {Interval}s", _interval.TotalSeconds);

        using var timer = new PeriodicTimer(_interval);

        try
        {
            do
            {
                await RunCycleAsync(stoppingToken);
            } while (await timer.WaitForNextTickAsync(stoppingToken));
        }
        catch (OperationCanceledException)
        {

        }

        _logger.LogInformation("Health check worker stopped");

    }
    private async Task RunCycleAsync(CancellationToken ct)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();

            var checks = scope.ServiceProvider.GetRequiredService<IHealthCheckService>();
            var result = await checks.CheckAllAsync(ct);

            _logger.LogInformation(
                "Health check cycle: {Total} checked, {Online}, {Offline} offline",
                result.Total, result.Online, result.Offline
                );

            if (DateTime.UtcNow - _lastCleanup >= CleanupEvery)
            {
                var logs = scope.ServiceProvider.GetRequiredService<IHealthLogService>();
                var deleted = await logs.CleanupOldLogsAsync();
                _lastCleanup = DateTime.UtcNow;

                _logger.LogInformation("Old health logs cleanup: {Deleted} records removed", deleted);
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check cycle failed");
        }
    }
}
