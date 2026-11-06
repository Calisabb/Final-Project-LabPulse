using LabPulse.BLL.DTOs.HealthChecks;
using LabPulse.DAL.DataContext.Entities;

namespace LabPulse.BLL.Services.Contracts;

public interface INetworkProbeService
{
    Task<ProbeResultDto> ProbeAsync(Service service, CancellationToken cancellationToken = default);
}
