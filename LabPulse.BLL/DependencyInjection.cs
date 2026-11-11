using LabPulse.BLL.Services.Contracts;
using LabPulse.BLL.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace LabPulse.BLL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddLabPulseBLL(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IServiceCatalogService, ServiceCatalogService>();
            services.AddScoped<IRunbookService, RunbookService>();
            services.AddScoped<IIncidentService, IncidentService>();
            services.AddScoped<IHealthCheckService, HealthCheckService>();
            services.AddScoped<INetworkProbeService, NetworkProbeService>();
            return services;
        }
    }
}
