using LabPulse.BLL.DTOs.Categories;

namespace LabPulse.BLL.Services.Contracts;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<CategoryDto> GetByIdAsync(int id);
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
    Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto);
    Task DeleteAsync(int id);
}