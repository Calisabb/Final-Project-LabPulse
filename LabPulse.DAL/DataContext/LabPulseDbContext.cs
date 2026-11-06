using LabPulse.DAL.DataContext.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LabPulse.DAL.DataContext;
public class LabPulseDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public LabPulseDbContext(DbContextOptions<LabPulseDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<HealthLog> HealthLogs => Set<HealthLog>();
    public DbSet<Incident> Incidents => Set<Incident>();
    public DbSet<Runbook> Runbooks => Set<Runbook>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(LabPulseDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var e in ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added))
        e.Entity.CreatedAt = DateTime.UtcNow;
        return base.SaveChangesAsync(ct);
    }
}
