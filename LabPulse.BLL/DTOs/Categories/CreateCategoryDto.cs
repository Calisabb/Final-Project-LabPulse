using System.ComponentModel.DataAnnotations;

namespace LabPulse.BLL.DTOs.Categories;

public class CreateCategoryDto
{
    [Required, StringLength(50)]
    public string Name { get; set; } = null!;
    [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid color format.")]
    public string? Color { get; set; }
}

