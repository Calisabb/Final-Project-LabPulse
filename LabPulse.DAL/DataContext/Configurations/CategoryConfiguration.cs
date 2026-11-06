using LabPulse.DAL.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LabPulse.DAL.DataContext.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> b)
    {
        b.Property(x => x.Name).IsRequired().HasMaxLength(50);
        b.HasIndex(x => x.Name).IsUnique();

        var seedDate = new DateTime(2026, 10, 25, 0, 0, 0, DateTimeKind.Utc);

        b.HasData(
            new Category { Id = 1, Name = "Dev Environment", Color = "#3B82F6", CreatedAt = seedDate },
            new Category { Id = 2, Name = "Databases", Color = "#10B981", CreatedAt = seedDate },
            new Category { Id = 3, Name = "Media", Color = "#F59E0B", CreatedAt = seedDate },
            new Category { Id = 4, Name = "Network Gateways", Color = "#8B5CF6", CreatedAt = seedDate }
        );
    }
}
