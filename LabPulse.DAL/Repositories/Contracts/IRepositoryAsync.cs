using LabPulse.DAL.DataContext.Entities;
using System.Linq.Expressions;

namespace LabPulse.DAL.Repositories.Contracts;

public interface IRepositoryAsync<T> where T : BaseEntity
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdASync();
    Task AddAsync(T entity);
    void Update(T entity);
    Task<bool> Delete(int id);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<int> SaveChangesAsync();
}
