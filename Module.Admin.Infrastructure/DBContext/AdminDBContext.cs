using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Module.Admin.Core.DBOs;
using Shared.Infrastructure.DBContext;
using Shared.Infrastructure.Models;

namespace Module.Admin.Infrastructure.DBContext;

public class AdminDBContext : BaseDBContext, IAdminDBContext
{
    public AdminDBContext(IOptions<DatabaseConnection> options) : base(options) { }

    public async Task<StatsDbo?> GetDashboardStatsAsync() =>
        await QuerySingleAsync("sp_GetDashboardStats", null, r => new StatsDbo
        {
            TotalPatients       = r.GetInt32(r.GetOrdinal("TotalPatients")),
            TotalAppointments   = r.GetInt32(r.GetOrdinal("TotalAppointments")),
            PendingAppointments = r.GetInt32(r.GetOrdinal("PendingAppointments")),
            CompletedToday      = r.GetInt32(r.GetOrdinal("CompletedToday")),
            ActiveRiders        = r.GetInt32(r.GetOrdinal("ActiveRiders")),
            RevenueThisMonth    = r.GetDecimal(r.GetOrdinal("RevenueThisMonth"))
        });

    public async Task<List<AppointmentAdminDbo>> GetAppointmentsAsync(
        int page, int perPage, string? search) =>
        await QueryListAsync("sp_GetAppointmentsPaged",
        [
            new SqlParameter("@Page",    page),
            new SqlParameter("@PerPage", perPage),
            new SqlParameter("@Search",  (object?)search ?? DBNull.Value)
        ],
        r => new AppointmentAdminDbo
        {
            AppointmentId  = r.GetInt32(r.GetOrdinal("AppointmentId")),
            UserId         = r.GetInt32(r.GetOrdinal("UserId")),
            PatientName    = r.GetString(r.GetOrdinal("PatientName")),
            Email          = r.GetString(r.GetOrdinal("Email")),
            TestName       = r.GetString(r.GetOrdinal("TestName")),
            Status         = r.GetInt32(r.GetOrdinal("Status")),
            ScheduledDate  = r.GetDateTime(r.GetOrdinal("ScheduledDate")),
            Address        = r.GetString(r.GetOrdinal("Address")),
            RiderName      = ReadNullableString(r, "RiderName"),
            RiderId        = ReadNullable<int>(r, "RiderId"),
            ReportFileName = ReadNullableString(r, "ReportFileName"),
            TotalCount     = r.GetInt32(r.GetOrdinal("TotalCount"))
        });

    public async Task UpdateAppointmentStatusAsync(int appointmentId, int status) =>
        await ExecuteAsync("sp_UpdateAppointmentStatus",
        [
            new SqlParameter("@AppointmentId", appointmentId),
            new SqlParameter("@Status",        status)
        ]);

    public async Task BulkUpdateStatusAsync(List<int> appointmentIds, int status)
    {
        // Pass IDs as comma-separated string, split in stored proc
        var ids = string.Join(",", appointmentIds);
        await ExecuteAsync("sp_BulkUpdateStatus",
        [
            new SqlParameter("@AppointmentIds", ids),
            new SqlParameter("@Status",         status)
        ]);
    }

    public async Task AssignRiderAsync(int appointmentId, int riderId) =>
        await ExecuteAsync("sp_AssignRider",
        [
            new SqlParameter("@AppointmentId", appointmentId),
            new SqlParameter("@RiderId",       riderId)
        ]);

    public async Task<int?> AutoAssignRiderAsync(int appointmentId) =>
        await ExecuteScalarAsync<int>("sp_AutoAssignRider",
        [new SqlParameter("@AppointmentId", appointmentId)]);

    public async Task SaveReportPathAsync(int appointmentId, string reportFileName) =>
        await ExecuteAsync("sp_SaveReportPath",
        [
            new SqlParameter("@AppointmentId",  appointmentId),
            new SqlParameter("@ReportFileName", reportFileName)
        ]);

    public async Task<List<PatientAdminDbo>> GetAllPatientsAsync() =>
        await QueryListAsync("sp_GetAllPatients", null, r => new PatientAdminDbo
        {
            UserId        = r.GetInt32(r.GetOrdinal("UserId")),
            Username      = r.GetString(r.GetOrdinal("Username")),
            Email         = r.GetString(r.GetOrdinal("Email")),
            TotalBookings = r.GetInt32(r.GetOrdinal("TotalBookings")),
            CreatedAt     = r.GetDateTime(r.GetOrdinal("CreatedAt"))
        });

    public async Task<List<TestAdminDbo>> GetAllTestsAsync() =>
        await QueryListAsync("sp_GetAllTests", null, r => new TestAdminDbo
        {
            TestId      = r.GetInt32(r.GetOrdinal("TestId")),
            Name        = r.GetString(r.GetOrdinal("Name")),
            Description = r.GetString(r.GetOrdinal("Description")),
            Price       = r.GetDecimal(r.GetOrdinal("Price")),
            Duration    = r.GetInt32(r.GetOrdinal("Duration")),
            IsActive    = r.GetBoolean(r.GetOrdinal("IsActive")),
            CreatedAt   = r.GetDateTime(r.GetOrdinal("CreatedAt"))
        });

    public async Task CreateTestAsync(
        string name, string description, decimal price, int duration) =>
        await ExecuteAsync("sp_CreateTest",
        [
            new SqlParameter("@Name",        name),
            new SqlParameter("@Description", description),
            new SqlParameter("@Price",       price),
            new SqlParameter("@Duration",    duration)
        ]);

    public async Task UpdateTestAsync(
        int testId, string name, string description, decimal price, int duration) =>
        await ExecuteAsync("sp_UpdateTest",
        [
            new SqlParameter("@TestId",      testId),
            new SqlParameter("@Name",        name),
            new SqlParameter("@Description", description),
            new SqlParameter("@Price",       price),
            new SqlParameter("@Duration",    duration)
        ]);

    public async Task DeleteTestAsync(int testId) =>
        await ExecuteAsync("sp_DeleteTest",
        [new SqlParameter("@TestId", testId)]);

    public async Task<List<RiderAdminDbo>> GetAllRidersAsync() =>
        await QueryListAsync("sp_GetAllRiders", null, MapRider);

    public async Task<RiderAdminDbo?> GetRiderByIdAsync(int riderId) =>
        await QuerySingleAsync("sp_GetRiderById",
        [new SqlParameter("@RiderId", riderId)], MapRider);

    public async Task CreateRiderAsync(
        string name, string email, string phone, string passwordHash) =>
        await ExecuteAsync("sp_CreateRider",
        [
            new SqlParameter("@Name",         name),
            new SqlParameter("@Email",        email),
            new SqlParameter("@Phone",        phone),
            new SqlParameter("@PasswordHash", passwordHash)
        ]);

    public async Task DeleteRiderAsync(int riderId) =>
        await ExecuteAsync("sp_DeleteRider",
        [new SqlParameter("@RiderId", riderId)]);

    private static RiderAdminDbo MapRider(SqlDataReader r) => new()
    {
        RiderId             = r.GetInt32(r.GetOrdinal("RiderId")),
        Name                = r.GetString(r.GetOrdinal("Name")),
        Email               = r.GetString(r.GetOrdinal("Email")),
        Phone               = r.GetString(r.GetOrdinal("Phone")),
        Status              = r.GetInt32(r.GetOrdinal("Status")),
        LastLatitude        = ReadNullable<double>(r, "LastLatitude"),
        LastLongitude       = ReadNullable<double>(r, "LastLongitude"),
        TotalTasksCompleted = r.GetInt32(r.GetOrdinal("TotalTasksCompleted")),
        CreatedAt           = r.GetDateTime(r.GetOrdinal("CreatedAt"))
    };
}
