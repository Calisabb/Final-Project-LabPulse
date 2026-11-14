using LabPulse.DAL.DataContext;
using LabPulse.DAL.DataContext.Entities;
using LabPulse.DAL.DataContext.Enums;
using LabPulse.DAL.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace LabPulse.DAL.Repositories.Implementations;

internal class ServiceRepositoryAsync : RepositoryAsync<Service>, IServiceRepositoryAsync
{
    public ServiceRepositoryAsync(LabPulseDbContext context) : base(context)
    {
    }

    public async Task<List<Service>> GetAllByCategoryAsync(ServiceStatus? status, int? categoryId, string? search)
    {
        var query = _dbSet.Include(s => s.Category).AsNoTracking().AsQueryable();
        if (status.HasValue)
            query = query.Where(s => s.ServiceStatus == status.Value);
        if (categoryId.HasValue)
            query = query.Where(s => s.CategoryId == categoryId.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s => s.Name.Contains(search) || (s.Description != null && s.Description.Contains(search)));
        }
        return await query.OrderBy(s => s.Name).ToListAsync();
    }

    public async Task<Service?> GetByIdWithDetailsAsync(int id)
    {
        var services = await _dbSet.Include(s => s.Category)
            .Include(s => s.Incidents)
            .Include(s => s.Runbooks)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
        return services;
    }

    public async Task<List<Service>> GetMonitoredAsync()
    {
        var rservices = await _dbSet.Where(s => s.IsMonitoringEnabled).ToListAsync();
        return rservices;
    }
}