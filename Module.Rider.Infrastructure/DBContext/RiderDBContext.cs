using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Module.Rider.Core.DBOs;
using Shared.Infrastructure.DBContext;
using Shared.Infrastructure.Models;

namespace Module.Rider.Infrastructure.DBContext;

public class RiderDBContext : BaseDBContext, IRiderDBContext
{
    public RiderDBContext(IOptions<DatabaseConnection> options) : base(options) { }

    public async Task<RiderDbo?> GetRiderByEmailAsync(string email) =>
        await QuerySingleAsync("sp_GetRiderByEmail",
        [new SqlParameter("@Email", email)], MapRider);

    public async Task<RiderDbo?> GetRiderByIdAsync(int riderId) =>
        await QuerySingleAsync("sp_GetRiderById",
        [new SqlParameter("@RiderId", riderId)], MapRider);

    public async Task<List<TaskDbo>> GetActiveTasksAsync(int riderId) =>
        await QueryListAsync("sp_GetActiveTasksByRider",
        [new SqlParameter("@RiderId", riderId)], MapTask);

    public async Task<List<TaskDbo>> GetTaskHistoryAsync(int riderId) =>
        await QueryListAsync("sp_GetTaskHistoryByRider",
        [new SqlParameter("@RiderId", riderId)], MapTask);

    public async Task<TaskDbo?> GetTaskByIdAsync(int appointmentId, int riderId) =>
        await QuerySingleAsync("sp_GetTaskByIdAndRider",
        [
            new SqlParameter("@AppointmentId", appointmentId),
            new SqlParameter("@RiderId",       riderId)
        ], MapTask);

    public async Task UpdateAppointmentStatusAsync(int appointmentId, int status) =>
        await ExecuteAsync("sp_UpdateAppointmentStatus",
        [
            new SqlParameter("@AppointmentId", appointmentId),
            new SqlParameter("@Status",        status)
        ]);

    public async Task InsertTaskLogAsync(int appointmentId, int riderId,
        string action, string? notes, string? photoPath,
        double? latitude, double? longitude) =>
        await ExecuteAsync("sp_InsertTaskLog",
        [
            new SqlParameter("@AppointmentId", appointmentId),
            new SqlParameter("@RiderId",       riderId),
            new SqlParameter("@Action",        action),
            new SqlParameter("@Notes",         (object?)notes     ?? DBNull.Value),
            new SqlParameter("@PhotoPath",     (object?)photoPath ?? DBNull.Value),
            new SqlParameter("@Latitude",      (object?)latitude  ?? DBNull.Value),
            new SqlParameter("@Longitude",     (object?)longitude ?? DBNull.Value)
        ]);

    public async Task UpdateRiderLocationAsync(
        int riderId, double latitude, double longitude, int status) =>
        await ExecuteAsync("sp_UpdateRiderLocation",
        [
            new SqlParameter("@RiderId",   riderId),
            new SqlParameter("@Latitude",  latitude),
            new SqlParameter("@Longitude", longitude),
            new SqlParameter("@Status",    status)
        ]);

    public async Task<List<NotificationDbo>> GetNotificationsAsync(int riderId) =>
        await QueryListAsync("sp_GetRiderNotifications",
        [new SqlParameter("@RiderId", riderId)],
        r => new NotificationDbo
        {
            NotificationId = r.GetInt32(r.GetOrdinal("NotificationId")),
            Title          = r.GetString(r.GetOrdinal("Title")),
            Body           = r.GetString(r.GetOrdinal("Body")),
            IsRead         = r.GetBoolean(r.GetOrdinal("IsRead")),
            CreatedAt      = r.GetDateTime(r.GetOrdinal("CreatedAt"))
        });

    public async Task MarkNotificationReadAsync(int notificationId, int riderId) =>
        await ExecuteAsync("sp_MarkNotificationRead",
        [
            new SqlParameter("@NotificationId", notificationId),
            new SqlParameter("@RiderId",        riderId)
        ]);

    public async Task MarkAllNotificationsReadAsync(int riderId) =>
        await ExecuteAsync("sp_MarkAllNotificationsRead",
        [new SqlParameter("@RiderId", riderId)]);

    private static RiderDbo MapRider(SqlDataReader r) => new()
    {
        RiderId        = r.GetInt32(r.GetOrdinal("RiderId")),
        Name           = r.GetString(r.GetOrdinal("Name")),
        Email          = r.GetString(r.GetOrdinal("Email")),
        Phone          = r.GetString(r.GetOrdinal("Phone")),
        PasswordHash   = r.GetString(r.GetOrdinal("PasswordHash")),
        PhotoPath      = ReadNullableString(r, "PhotoPath"),
        Status         = r.GetInt32(r.GetOrdinal("Status")),
        LastLatitude   = ReadNullable<double>(r, "LastLatitude"),
        LastLongitude  = ReadNullable<double>(r, "LastLongitude"),
        LastLocationAt = ReadNullable<DateTime>(r, "LastLocationAt"),
        CreatedAt      = r.GetDateTime(r.GetOrdinal("CreatedAt"))
    };

    private static TaskDbo MapTask(SqlDataReader r) => new()
    {
        AppointmentId = r.GetInt32(r.GetOrdinal("AppointmentId")),
        UserId        = r.GetInt32(r.GetOrdinal("UserId")),
        PatientName   = r.GetString(r.GetOrdinal("PatientName")),
        PatientPhone  = r.GetString(r.GetOrdinal("PatientPhone")),
        TestName      = r.GetString(r.GetOrdinal("TestName")),
        Address       = r.GetString(r.GetOrdinal("Address")),
        Latitude      = ReadNullable<double>(r, "Latitude"),
        Longitude     = ReadNullable<double>(r, "Longitude"),
        Status        = r.GetInt32(r.GetOrdinal("Status")),
        ScheduledDate = r.GetDateTime(r.GetOrdinal("ScheduledDate"))
    };
}
