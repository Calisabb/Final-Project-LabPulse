using LabPulse.DAL.DataContext.Entities;
using LabPulse.DAL.DataContext.Enums;

namespace LabPulse.DAL.Repositories.Contracts;

public interface IServiceRepositoryAsync : IRepositoryAsync<Service>
{
    Task<List<Service>> GetAllByCategoryAsync(ServiceStatus? status, int? categoryId, string? search);
    Task<Service?> GetByIdWithDetailsAsync(int id);
    Task<List<Service>> GetMonitoredAsync();
}
