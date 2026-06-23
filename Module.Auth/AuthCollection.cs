using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module.Auth.Core.Services;
using Module.Auth.Infrastructure.DBContext;

namespace Module.Auth;

public static class AuthCollection
{
    public static IServiceCollection AddModuleAuth(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IAuthDBContext, AuthDBContext>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
