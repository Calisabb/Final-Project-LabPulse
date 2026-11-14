using LabPulse.DAL.DataContext.Entities;

namespace LabPulse.DAL.Repositories.Contracts;

public interface IIncidentRepositoryAsync : IRepositoryAsync<Incident>
{
    Task<Incident?> GetOpenByServiceAsync(int serviceId);
    Task<List<Incident>> GetAllWithServiceAsync(bool onlyOpen, int? serviceId = null); 
}
