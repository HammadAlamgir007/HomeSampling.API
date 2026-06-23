using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module.Contact.Core.Services;

namespace Module.Contact;

public static class ContactCollection
{
    public static IServiceCollection AddModuleContact(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IContactService, ContactService>();
        return services;
    }
}
