using LabPulse.DAL.DataContext;
using LabPulse.DAL.Repositories.Contracts;
using LabPulse.DAL.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LabPulse.DAL;

public static class DependencyInjection
{
    public static IServiceCollection AddLabPulseDAL(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LabPulseDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default")));
        services.AddScoped(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));
        return services;
    }
}
