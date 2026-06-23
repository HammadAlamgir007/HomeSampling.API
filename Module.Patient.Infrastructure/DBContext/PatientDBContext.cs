using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Module.Patient.Core.DBOs;
using Shared.Infrastructure.DBContext;
using Shared.Infrastructure.Models;

namespace Module.Patient.Infrastructure.DBContext;

public class PatientDBContext : BaseDBContext, IPatientDBContext
{
    public PatientDBContext(IOptions<DatabaseConnection> options) : base(options) { }

    public async Task<List<TestDbo>> GetActiveTestsAsync() =>
        await QueryListAsync("sp_GetActiveTests", null, r => new TestDbo
        {
            TestId      = r.GetInt32(r.GetOrdinal("TestId")),
            Name        = r.GetString(r.GetOrdinal("Name")),
            Description = r.GetString(r.GetOrdinal("Description")),
            Price       = r.GetDecimal(r.GetOrdinal("Price")),
            Duration    = r.GetInt32(r.GetOrdinal("Duration")),
            IsActive    = r.GetBoolean(r.GetOrdinal("IsActive")),
            CreatedAt   = r.GetDateTime(r.GetOrdinal("CreatedAt"))
        });

    public async Task<AppointmentDbo?> GetAppointmentByDateAsync(
        int userId, int testId, DateTime date) =>
        await QuerySingleAsync("sp_GetAppointmentByDate",
        [
            new SqlParameter("@UserId", userId),
            new SqlParameter("@TestId", testId),
            new SqlParameter("@Date",   date.Date)
        ], MapAppointment);

    public async Task<int> CreateBookingAsync(
        int userId, int testId, DateTime scheduledDate,
        string address, double? latitude, double? longitude)
    {
        var result = await ExecuteScalarAsync<int>("sp_CreateBooking",
        [
            new SqlParameter("@UserId",        userId),
            new SqlParameter("@TestId",        testId),
            new SqlParameter("@ScheduledDate", scheduledDate),
            new SqlParameter("@Address",       address),
            new SqlParameter("@Latitude",      (object?)latitude  ?? DBNull.Value),
            new SqlParameter("@Longitude",     (object?)longitude ?? DBNull.Value)
        ]);
        return result;
    }

    public async Task<List<AppointmentDbo>> GetPatientBookingsAsync(int userId) =>
        await QueryListAsync("sp_GetPatientBookings",
        [new SqlParameter("@UserId", userId)],
        MapAppointment);

    public async Task<AppointmentDbo?> GetAppointmentByIdAsync(
        int appointmentId, int userId) =>
        await QuerySingleAsync("sp_GetAppointmentById",
        [
            new SqlParameter("@AppointmentId", appointmentId),
            new SqlParameter("@UserId",        userId)
        ], MapAppointment);

    public async Task CancelBookingAsync(int appointmentId) =>
        await ExecuteAsync("sp_CancelBooking",
        [new SqlParameter("@AppointmentId", appointmentId)]);

    public async Task<AppointmentDbo?> GetAppointmentByReportAsync(
        string reportFileName, int userId) =>
        await QuerySingleAsync("sp_GetAppointmentByReport",
        [
            new SqlParameter("@ReportFileName", reportFileName),
            new SqlParameter("@UserId",         userId)
        ], MapAppointment);

    private static AppointmentDbo MapAppointment(SqlDataReader r) => new()
    {
        AppointmentId    = r.GetInt32(r.GetOrdinal("AppointmentId")),
        UserId           = r.GetInt32(r.GetOrdinal("UserId")),
        TestId           = r.GetInt32(r.GetOrdinal("TestId")),
        TestName         = r.GetString(r.GetOrdinal("TestName")),
        RiderId          = ReadNullable<int>(r, "RiderId"),
        Status           = r.GetInt32(r.GetOrdinal("Status")),
        ScheduledDate    = r.GetDateTime(r.GetOrdinal("ScheduledDate")),
        Address          = r.GetString(r.GetOrdinal("Address")),
        Latitude         = ReadNullable<double>(r, "Latitude"),
        Longitude        = ReadNullable<double>(r, "Longitude"),
        ReportFileName   = ReadNullableString(r, "ReportFileName"),
        ReportUploadedAt = ReadNullable<DateTime>(r, "ReportUploadedAt"),
        CreatedAt        = r.GetDateTime(r.GetOrdinal("CreatedAt"))
    };
}
