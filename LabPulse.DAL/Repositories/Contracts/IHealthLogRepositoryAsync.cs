using LabPulse.DAL.DataContext.Entities;
using LabPulse.DAL.Repositories.Models;

namespace LabPulse.DAL.Repositories.Contracts;

public interface IHealthLogRepositoryAsync
{
    Task AddAsync(HealthLog log);
    Task<List<HealthLog>> GetAllByServiceIdAsync(int serviceId, DateTime from, DateTime to);
    Task<List<HealthLog>> GetLastAsync(int serviceId, int count);
    Task<int> DeleteOlderThanAsync(DateTime cutoffDate);
    Task<int> SaveChangesAsync();
    Task<List<DailyHealthStat>> GetDailyStatsAsync(int serviceId, DateTime from);
}