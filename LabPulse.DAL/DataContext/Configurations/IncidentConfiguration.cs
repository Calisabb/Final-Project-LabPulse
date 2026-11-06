using LabPulse.DAL.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LabPulse.DAL.DataContext.Configurations;

public class IncidentConfiguration : IEntityTypeConfiguration<Incident>
{
    public void Configure(EntityTypeBuilder<Incident> b)
    {
        b.HasOne(x => x.Service).WithMany(c => c.Incidents)
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => x.ServiceId).IsUnique().HasFilter("[ResolvedAt] IS NULL");
    }
}
