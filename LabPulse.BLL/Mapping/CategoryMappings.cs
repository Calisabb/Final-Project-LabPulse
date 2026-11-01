using LabPulse.BLL.DTOs.Categories;
using LabPulse.DAL.DataContext.Entities;

namespace LabPulse.BLL.Mapping;

public static class CategoryMappings
{
    public static CategoryDto ToDto(this Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Color = category.Color
        };
    }
    public static Category ToEntity(this CreateCategoryDto dto)
    {
        return new Category
        {
            Name = dto.Name.Trim(),
            Color = dto.Color
        };
    }
    public static void ApplyTo(this UpdateCategoryDto dto, Category category)
    {
        category.Name = dto.Name.Trim();
        category.Color = dto.Color;
    }
}