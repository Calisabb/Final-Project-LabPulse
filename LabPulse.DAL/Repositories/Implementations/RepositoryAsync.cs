using LabPulse.DAL.DataContext;
using LabPulse.DAL.DataContext.Entities;
using LabPulse.DAL.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LabPulse.DAL.Repositories.Implementations;

public class RepositoryAsync<T> : IRepositoryAsync<T> where T : BaseEntity
{
    protected readonly LabPulseDbContext _dbContext;
    protected readonly DbSet<T> _dbSet;

    public RepositoryAsync(LabPulseDbContext context)
    {
        _dbContext = context;
        _dbSet = context.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> Delete(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            return true;
        }
        return false;
    }

    public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.AnyAsync(predicate);
    }

    public async Task<List<T>> GetAllAsync()
    {
        var entities = await _dbSet.ToListAsync();
        return entities;
    }

    public async Task<T?> GetByIdASync(int id)
    {
        return await _dbSet.FindAsync(id);
    }
    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public async Task<T?> GetByIdASync()
    {
        var entity = await _dbSet.FindAsync();
        return entity;
    }
}
