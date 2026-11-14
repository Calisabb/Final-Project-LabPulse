using LabPulse.BLL.DTOs.Incidents;

namespace LabPulse.BLL.Services.Contracts;

public interface IIncidentService
{
    Task<List<IncidentDto>> GetAllAsync(bool onlyOpen);
    Task<List<IncidentDto>?> GetByServiceAsync(int serviceId);
}
