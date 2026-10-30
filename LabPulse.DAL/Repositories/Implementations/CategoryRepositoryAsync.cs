using LabPulse.DAL.DataContext;
using LabPulse.DAL.DataContext.Entities;
using LabPulse.DAL.Repositories.Contracts;

namespace LabPulse.DAL.Repositories.Implementations;

public class CategoryRepositoryAsync : RepositoryAsync<Category>, ICategoryRepositoryAsync
{
    public CategoryRepositoryAsync(LabPulseDbContext context) : base(context)
    {
    }
}
