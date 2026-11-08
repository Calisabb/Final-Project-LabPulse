using LabPulse.DAL.DataContext;
using LabPulse.DAL.DataContext.Entities;
using LabPulse.DAL.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace LabPulse.DAL.Repositories.Implementations;

internal class RunbookRepositoryAsync : RepositoryAsync<Runbook>, IRunbookRepositoryAsync
{
    public RunbookRepositoryAsync(LabPulseDbContext context) : base(context)
    {
    }

    public async Task<List<Runbook>> GetAllByServiceIdAsync(int serviceId)
    {
        var query = await _dbSet.Where(r => r.ServiceId == serviceId).ToListAsync();
        return query;
    }
}
