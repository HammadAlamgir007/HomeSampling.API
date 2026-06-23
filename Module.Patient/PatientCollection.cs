using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module.Patient.Core.Services;
using Module.Patient.Infrastructure.DBContext;

namespace Module.Patient;

public static class PatientCollection
{
    public static IServiceCollection AddModulePatient(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IPatientDBContext, PatientDBContext>();
        services.AddScoped<IPatientService, PatientService>();
        return services;
    }
}
