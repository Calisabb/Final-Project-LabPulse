using LabPulse.BLL.DTOs.Incidents;
using LabPulse.BLL.Mapping;
using LabPulse.BLL.Services.Contracts;
using LabPulse.DAL.Repositories.Contracts;

namespace LabPulse.BLL.Services.Implementations;

internal class IncidentService : IIncidentService
{
    private readonly IIncidentRepositoryAsync _incidents;
    private readonly IServiceRepositoryAsync _services;

    public IncidentService(IIncidentRepositoryAsync incidents, IServiceRepositoryAsync services)
    {
        _incidents = incidents;
        _services = services;
    }

    public async Task<List<IncidentDto>> GetAllAsync(bool onlyOpen)
    {
        var incidents = await _incidents.GetAllWithServiceAsync(onlyOpen);
        return incidents.Select(i=>i.ToDto()).ToList();
    }

    public async Task<List<IncidentDto>?> GetByServiceAsync(int serviceId)
    {
        if (!await _services.ExistsAsync(s=>s.Id == serviceId))
            throw new KeyNotFoundException($"Service with id {serviceId} not found.");

        var incidents = await _incidents.GetAllWithServiceAsync(onlyOpen: false, serviceId: serviceId);
        return incidents.Select(i => i.ToDto()).ToList();
    }
}
