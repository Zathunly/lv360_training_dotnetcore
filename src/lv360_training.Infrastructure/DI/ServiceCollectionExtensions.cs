using lv360_training.Domain.Interfaces.Repositories.Auth;
using lv360_training.Domain.Interfaces.Repositories.Core;
using lv360_training.Domain.Interfaces.Repositories.Catalog;
using lv360_training.Domain.Interfaces.Security;
using lv360_training.Infrastructure.Repositories.Auth;
using lv360_training.Infrastructure.Repositories.Core;
using lv360_training.Infrastructure.Repositories.Catalog;
using lv360_training.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;
using lv360_training.Domain.Entities;

namespace lv360_training.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<ICatalogRepository<Category>, CatalogRepository<Category>>();
            services.AddScoped<ICatalogRepository<Product>, CatalogRepository<Product>>();
            services.AddScoped<ICatalogRepository<Stock>, CatalogRepository<Stock>>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
