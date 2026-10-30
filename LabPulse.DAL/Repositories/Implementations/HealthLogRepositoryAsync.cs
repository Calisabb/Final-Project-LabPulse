using LabPulse.DAL.DataContext;
using LabPulse.DAL.DataContext.Entities;
using LabPulse.DAL.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace LabPulse.DAL.Repositories.Implementations;

public class HealthLogRepositoryAsync : IHealthLogRepositoryAsync
{
    private readonly LabPulseDbContext _dbContext;
    public HealthLogRepositoryAsync(LabPulseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(HealthLog log)
    {
        await _dbContext.HealthLogs.AddAsync(log);
    }
    public async Task<List<HealthLog>> GetAllByServiceIdAsync(int serviceId, DateTime from, DateTime to)
    {
        return await _dbContext.HealthLogs
            .Where(h => h.ServiceId == serviceId && h.CheckedAt >= from && h.CheckedAt <= to)
            .OrderBy(h => h.CheckedAt)
            .ToListAsync();
    }

    public async Task<int> DeleteOlderThanAsync(DateTime cutoffDate)
    {
        return await _dbContext.HealthLogs
            .Where(h => h.CheckedAt < cutoffDate)
            .ExecuteDeleteAsync();
    }
            

    public async Task<List<HealthLog>> GetLastAsync(int serviceId, int count)
    {
        return await _dbContext.HealthLogs
            .Where(h => h.ServiceId == serviceId)
            .OrderByDescending(h => h.CheckedAt)
            .Take(count)
            .ToListAsync();
    }

    public Task<int> SaveChangesAsync()
    {
        return _dbContext.SaveChangesAsync();
    }
}
