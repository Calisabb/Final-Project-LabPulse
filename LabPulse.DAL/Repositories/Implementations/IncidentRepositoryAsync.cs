using LabPulse.DAL.DataContext;
using LabPulse.DAL.DataContext.Entities;
using LabPulse.DAL.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace LabPulse.DAL.Repositories.Implementations;

public class IncidentRepositoryAsync : RepositoryAsync<Incident>, IIncidentRepositoryAsync
{
    public IncidentRepositoryAsync(LabPulseDbContext context) : base(context)
    {
    }

    public async Task<List<Incident>> GetAllWithServiceAsync(bool onlyOpen, int? serviceId = null)
    {
        var incidents = _dbSet.AsNoTracking().Include(i => i.Service).AsQueryable();
        if (onlyOpen)
        {
            incidents = incidents.Where(i => i.ResolvedAt == null);
        }
        if (serviceId.HasValue)
        {
            incidents = incidents.Where(i => i.ServiceId == serviceId.Value);
        }
        return await incidents.OrderByDescending(i => i.StartedAt).ToListAsync();
    }

    public async Task<Incident?> GetOpenByServiceAsync(int serviceId)
    {
        return await _dbSet.FirstOrDefaultAsync(i => i.ServiceId == serviceId && i.ResolvedAt == null);
    }
}
