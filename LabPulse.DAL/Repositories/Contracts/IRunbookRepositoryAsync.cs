using LabPulse.DAL.DataContext.Entities;

namespace LabPulse.DAL.Repositories.Contracts;

public interface IRunbookRepositoryAsync : IRepositoryAsync<Runbook>
{
    Task<List<Runbook>> GetAllByServiceIdAsync(int serviceId);
}
