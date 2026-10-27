using LabPulse.DAL.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LabPulse.DAL.DataContext.Configurations;

public class HealthLogConfiguration : IEntityTypeConfiguration<HealthLog>
{
    public void Configure(EntityTypeBuilder<HealthLog> b)
    {
        b.HasOne(x=>x.Service).WithMany(c => c.HealthLogs)
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => new { x.ServiceId, x.CheckedAt });
        b.Property(x => x.ErrorMessage).HasMaxLength(500);
    }
}
