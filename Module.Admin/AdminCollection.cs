using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module.Admin.Core.Services;
using Module.Admin.Infrastructure.DBContext;

namespace Module.Admin;

public static class AdminCollection
{
    public static IServiceCollection AddModuleAdmin(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IAdminDBContext, AdminDBContext>();
        services.AddScoped<IAdminService, AdminService>();
        return services;
    }
}
