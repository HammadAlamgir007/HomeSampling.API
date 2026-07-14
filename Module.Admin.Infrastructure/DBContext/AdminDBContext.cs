
using Microsoft.Extensions.Options;
using Module.Admin.Core.DBContext;
using Module.Admin.Core.DBOs;
using Shared.Infrastructure.DBContext;
using Shared.Infrastructure.Models;


namespace Module.Admin.Infrastructure.DBContext;

public class AdminDBContext : BaseDBContext, IAdminDBContext
{
    public AdminDBContext(IOptions<DatabaseConnection> options) : base(options) { }

    public async Task<StatsDbo?> GetDashboardStatsAsync()
    {
        return await QuerySingleAsync<StatsDbo>("sp_GetDashboardStats");
    }
    public async Task<List<AppointmentAdminDbo>> GetAppointmentsAsync(
        int page, int perPage, string? search)
    {
        return await QueryListAsync<AppointmentAdminDbo>("sp_GetAppointmentsPaged",
            new
            {
                Page = page,
                PerPage = perPage,
                Search = search
            });
    }

    public async Task UpdateAppointmentStatusAsync(int appointmentId, int status)
    {
         await ExecuteAsync("sp_UpdateAppointmentStatus",
            new
            {
                AppointmentId = appointmentId,
                Status = status
            });
              }


    public async Task BulkUpdateStatusAsync(List<int> appointmentIds, int status)
    {
        var ids = string.Join(",", appointmentIds);

        await ExecuteAsync(
            "sp_BulkUpdateStatus",
            new
            {
                AppointmentIds = ids,
                Status = status
            });
    }

    public async Task AssignRiderAsync(int appointmentId, int riderId)
    {
        await ExecuteAsync(
            "sp_AssignRider",
            new
            {
                AppointmentId = appointmentId,
                RiderId = riderId
            });
    }
    public async Task<int?> AutoAssignRiderAsync(int appointmentId)
    {
        return await ExecuteScalarAsync<int>(
            "sp_AutoAssignRider",
            new
            {
                AppointmentId = appointmentId
            });
    }
    public async Task SaveReportPathAsync(int appointmentId, string reportFileName)
    {
        await ExecuteAsync(
            "sp_SaveReportPath",
            new
            {
                AppointmentId = appointmentId,
                ReportFileName = reportFileName
            });
    }
    public async Task<List<PatientAdminDbo>> GetAllPatientsAsync()
    {
        return await QueryListAsync<PatientAdminDbo>(
            "sp_GetAllPatients");
    }

    public async Task<List<TestAdminDbo>> GetAllTestsAsync()
    {
        return await QueryListAsync<TestAdminDbo>(
            "sp_GetAllTests");
    }
    public async Task CreateTestAsync(
     string name,
     string description,
     decimal price,
     int duration)
    {
        await ExecuteAsync(
            "sp_CreateTest",
            new
            {
                Name = name,
                Description = description,
                Price = price,
                Duration = duration
            });
    }

    public async Task UpdateTestAsync(
      int testId,
      string name,
      string description,
      decimal price,
      int duration)
    {
        await ExecuteAsync(
            "sp_UpdateTest",
            new
            {
                TestId = testId,
                Name = name,
                Description = description,
                Price = price,
                Duration = duration
            });
    }

    public async Task DeleteTestAsync(int testId)
    {
        await ExecuteAsync(
            "sp_DeleteTest",
            new
            {
                TestId = testId
            });
    }

    public async Task<List<RiderAdminDbo>> GetAllRidersAsync()
    {
        return await QueryListAsync<RiderAdminDbo>(
            "sp_GetAllRiders");
    }

    public async Task<RiderAdminDbo?> GetRiderByIdAsync(int riderId)
    {
        return await QuerySingleAsync<RiderAdminDbo>(
            "sp_GetRiderById",
            new
            {
                RiderId = riderId
            });
    }

    public async Task CreateRiderAsync(
      string name,
      string email,
      string phone,
      string passwordHash)
    {
        await ExecuteAsync(
            "sp_CreateRider",
            new
            {
                Name = name,
                Email = email,
                Phone = phone,
                PasswordHash = passwordHash
            });
    }

    public async Task DeleteRiderAsync(int riderId)
    {
        await ExecuteAsync(
            "sp_DeleteRider",
            new
            {
                RiderId = riderId
            });
    }

}
