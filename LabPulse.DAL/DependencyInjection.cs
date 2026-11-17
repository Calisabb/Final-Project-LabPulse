using LabPulse.DAL.DataContext;
using LabPulse.DAL.DataContext.Entities;
using LabPulse.DAL.Repositories.Contracts;
using LabPulse.DAL.Repositories.Implementations;
using Microsoft.AspNetCore.Identity;
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

        services.AddIdentityCore<User>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<LabPulseDbContext>();

        services.AddScoped(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));
        services.AddScoped<ICategoryRepositoryAsync, CategoryRepositoryAsync>();
        services.AddScoped<IRunbookRepositoryAsync, RunbookRepositoryAsync>();
        services.AddScoped<IServiceRepositoryAsync, ServiceRepositoryAsync>();
        services.AddScoped<IHealthLogRepositoryAsync, HealthLogRepositoryAsync>();
        services.AddScoped<IIncidentRepositoryAsync, IncidentRepositoryAsync>();
        return services;
    }
}
