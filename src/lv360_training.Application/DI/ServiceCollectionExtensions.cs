using lv360_training.Application.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace lv360_training.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<AuthHandler>();
            services.AddScoped<AdminHandler>();
            services.AddScoped<ProductHandler>();

            return services;
        }
    }
}
