using LabPulse.BLL.DTOs.HealthChecks;

namespace LabPulse.BLL.Services.Contracts
{
    public interface IHealthCheckService
    {
        Task<ServiceCheckResultDto> CheckServiceAsync(int serviceId, CancellationToken ct = default);
        Task<BulkCheckResultDto> CheckAllAsync(CancellationToken ct = default);
        
    }
}
