using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Shared.Infrastructure.Models;
using Shared.Infrastructure.Services;

namespace Shared.Infrastructure;

public static class SharedCollection
{
    public static IServiceCollection AddSharedInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.Configure<DatabaseConnection>(
            config.GetSection("ConnectionStrings"));

        services.AddSingleton<IGuidService, GuidService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IFileService, FileService>();

        return services;
    }
}
