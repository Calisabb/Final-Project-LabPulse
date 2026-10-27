namespace LabPulse.DAL.DataContext.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Color { get; set; }
    public ICollection<Service> Services { get; set; } = new List<Service>();
}
