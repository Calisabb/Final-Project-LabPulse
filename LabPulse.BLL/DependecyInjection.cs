using LabPulse.BLL.Services.Contracts;
using LabPulse.BLL.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace LabPulse.BLL
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddLabPulseBLL(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IServiceCatalogService, ServiceCatalogService>();
            services.AddScoped<IRunbookService, RunbookService>();
            services.AddScoped<IIncidentService, IncidentService>();
            return services;
        }
    }
}
