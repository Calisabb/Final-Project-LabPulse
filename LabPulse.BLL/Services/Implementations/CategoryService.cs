using LabPulse.BLL.DTOs.Categories;
using LabPulse.BLL.Exceptions;
using LabPulse.BLL.Mapping;
using LabPulse.BLL.Services.Contracts;
using LabPulse.DAL.Repositories.Contracts;

namespace LabPulse.BLL.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepositoryAsync _categoryRepository;
    private readonly IServiceRepositoryAsync _serviceRepository;

    public CategoryService(ICategoryRepositoryAsync categoryRepository, IServiceRepositoryAsync serviceRepository)
    {
        _categoryRepository = categoryRepository;
        _serviceRepository = serviceRepository;
    }
    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        var name = dto.Name.Trim();
        if (await _categoryRepository.ExistsAsync(c => c.Name == name))
        {
            throw new InvalidOperationException("Category with this name already exists.");
        }

        var entity = dto.ToEntity();
        await _categoryRepository.AddAsync(entity);
        await _categoryRepository.SaveChangesAsync();
        return entity.ToDto();
    }

    public async Task DeleteAsync(int id)
    {
        if (await _serviceRepository.ExistsAsync(s => s.CategoryId == id))
        {
            throw new ConflictException("Category has services and cannot be deleted.");
        }
        if (!await _categoryRepository.Delete(id))
        {
            throw new KeyNotFoundException("Category not found.");
        }
        await _categoryRepository.SaveChangesAsync();
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var entities = await _categoryRepository.GetAllAsync();
        return entities.Select(c => c.ToDto()).ToList();
    }

    public async Task<CategoryDto> GetByIdAsync(int id)
    {
        var entity = await _categoryRepository.GetByIdAsync(id);
        if (entity == null)
        {
            throw new KeyNotFoundException("Category not found.");
        }
        return entity.ToDto();
    }

    public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        var entity = await _categoryRepository.GetByIdAsync(id);
        if (entity == null)
        {
            throw new KeyNotFoundException("Category not found.");
        }

        entity.Name = dto.Name.Trim();
        if (await _categoryRepository.ExistsAsync(c => c.Name == entity.Name && c.Id != id))
        {
            throw new InvalidOperationException("Category with this name already exists.");
        }
        dto.ApplyTo(entity);     
        await _categoryRepository.SaveChangesAsync();
        return entity.ToDto();
    }
}