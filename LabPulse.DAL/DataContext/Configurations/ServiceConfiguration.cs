using LabPulse.DAL.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LabPulse.DAL.DataContext.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> b)
    {
        b.Property(x => x.Name).IsRequired().HasMaxLength(100);
        b.Property(x => x.Host).IsRequired().HasMaxLength(255);
        b.Property(x => x.HealthPath).HasMaxLength(255);
        b.HasOne(x => x.Category).WithMany(c => c.Services)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}