using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module.Rider.Core.Services;
using Module.Rider.Infrastructure.DBContext;

namespace Module.Rider;

public static class RiderCollection
{
    public static IServiceCollection AddModuleRider(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IRiderDBContext, RiderDBContext>();
        services.AddScoped<IRiderService, RiderService>();
        return services;
    }
}
