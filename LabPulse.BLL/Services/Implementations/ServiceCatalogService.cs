using LabPulse.BLL.DTOs.Services;
using LabPulse.BLL.Exceptions;
using LabPulse.BLL.Mapping;
using LabPulse.BLL.Services.Contracts;
using LabPulse.DAL.DataContext.Enums;
using LabPulse.DAL.Repositories.Contracts;

namespace LabPulse.BLL.Services.Implementations;

public class ServiceCatalogService : IServiceCatalogService
{
    private readonly IServiceRepositoryAsync _services;
    private readonly ICategoryRepositoryAsync _categories;

    public ServiceCatalogService(IServiceRepositoryAsync services, ICategoryRepositoryAsync categories)
    {
        _services = services;
        _categories = categories;
    }

    public async Task<ServiceDto> CreateAsync(CreateServiceDto dto)
    {
        var category = await _categories.GetByIdAsync(dto.CategoryId);
        if (category == null)
        {
            throw new KeyNotFoundException($"Category with ID {dto.CategoryId} does not exist.");
        }
        var host = dto.Host.Trim();
        if (await _services.ExistsAsync(s=>s.Host == host && s.Port == dto.Port))
        {
            throw new ConflictException($"Service {host}:{dto.Port} already exists.");
        }
        var entity = dto.ToEntity();
        entity.Category = category;
        await _services.AddAsync(entity);
        await _services.SaveChangesAsync();
        return entity.ToDto();
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _services.Delete(id))
        {
            throw new KeyNotFoundException($"Service with ID {id} does not exist.");
        }
        await _services.SaveChangesAsync();
    }

    public async Task<List<ServiceDto>> GetAllAsync(ServiceStatus? status, int? categoryId, string? search)
    {
        var services = await _services.GetAllByCategoryAsync(status, categoryId, search);
        return services.Select(s => s.ToDto()).ToList();
    }

    public async Task<ServiceDto> GetByIdAsync(int id)
    {
        var service = await _services.GetByIdAsync(id);
        if (service == null)
        {
            throw new KeyNotFoundException($"Service with ID {id} does not exist.");
        }
        return service.ToDto();
    }

    public async Task<ServiceDto> UpdateAsync(int id, UpdateServiceDto dto)
    {
        var service = await _services.GetByIdAsync(id);
        if (service == null)
        {
            throw new KeyNotFoundException($"Service with ID {id} does not exist.");
        }
        var category = await _categories.GetByIdAsync(dto.CategoryId);
        if (category == null)
        {
            throw new KeyNotFoundException($"Category with ID {dto.CategoryId} does not exist.");
        }
        var host = dto.Host.Trim();
        if (await _services.ExistsAsync(s => s.Id != id && s.Host == host && s.Port == dto.Port))
        {
            throw new ConflictException($"Service {host}:{dto.Port} already exists.");
        }
        dto.ApplyTo(service);
        service.Category = category;
        await _services.SaveChangesAsync();
        return service.ToDto();
    }
}