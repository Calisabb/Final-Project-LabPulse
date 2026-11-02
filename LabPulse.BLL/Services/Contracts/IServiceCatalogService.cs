using LabPulse.BLL.DTOs.Services;
using LabPulse.DAL.DataContext.Enums;

namespace LabPulse.BLL.Services.Contracts
{
    public interface IServiceCatalogService
    {
        Task<List<ServiceDto>> GetAllAsync(ServiceStatus? status, int? categoryId, string? search);
        Task<ServiceDto> GetByIdAsync(int id);
        Task<ServiceDto> CreateAsync(CreateServiceDto dto);
        Task<ServiceDto> UpdateAsync(int id, UpdateServiceDto dto);
        Task DeleteAsync(int id);
    }
}