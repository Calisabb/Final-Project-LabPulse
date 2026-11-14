using LabPulse.DAL.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LabPulse.DAL.DataContext.Configurations;

public class RunbookConfiguration : IEntityTypeConfiguration<Runbook>
{
    public void Configure(EntityTypeBuilder<Runbook> b)
    {
        b.Property(x => x.Title).IsRequired().HasMaxLength(150);
        b.Property(x => x.CommandText).IsRequired();
        b.HasOne(x => x.Service).WithMany(c => c.Runbooks)
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}