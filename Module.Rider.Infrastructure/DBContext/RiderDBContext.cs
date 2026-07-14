using Microsoft.Extensions.Options;
using Module.Rider.Core.DBOs;
using Module.Rider.Core.DBContext;
using Shared.Infrastructure.DBContext;
using Shared.Infrastructure.Models;

namespace Module.Rider.Infrastructure.DBContext;

public class RiderDBContext : BaseDBContext, IRiderDBContext
{
    public RiderDBContext(IOptions<DatabaseConnection> options)
        : base(options)
    {
    }

    public async Task<RiderDbo?> GetRiderByEmailAsync(string email)
    {
        return await QuerySingleAsync<RiderDbo>(
            "sp_GetRiderByEmail",
            new
            {
                Email = email
            });
    }

    public async Task<RiderDbo?> GetRiderByIdAsync(int riderId)
    {
        return await QuerySingleAsync<RiderDbo>(
            "sp_GetRiderById",
            new
            {
                RiderId = riderId
            });
    }

    public async Task<List<TaskDbo>> GetActiveTasksAsync(int riderId)
    {
        return await QueryListAsync<TaskDbo>(
            "sp_GetActiveTasksByRider",
            new
            {
                RiderId = riderId
            });
    }

    public async Task<List<TaskDbo>> GetTaskHistoryAsync(int riderId)
    {
        return await QueryListAsync<TaskDbo>(
            "sp_GetTaskHistoryByRider",
            new
            {
                RiderId = riderId
            });
    }

    public async Task<TaskDbo?> GetTaskByIdAsync(int appointmentId, int riderId)
    {
        return await QuerySingleAsync<TaskDbo>(
            "sp_GetTaskByIdAndRider",
            new
            {
                AppointmentId = appointmentId,
                RiderId = riderId
            });
    }

    public async Task UpdateAppointmentStatusAsync(int appointmentId, int status)
    {
        await ExecuteAsync(
            "sp_UpdateAppointmentStatus",
            new
            {
                AppointmentId = appointmentId,
                Status = status
            });
    }

    public async Task InsertTaskLogAsync(
        int appointmentId,
        int riderId,
        string action,
        string? notes,
        string? photoPath,
        double? latitude,
        double? longitude)
    {
        await ExecuteAsync(
            "sp_InsertTaskLog",
            new
            {
                AppointmentId = appointmentId,
                RiderId = riderId,
                Action = action,
                Notes = notes,
                PhotoPath = photoPath,
                Latitude = latitude,
                Longitude = longitude
            });
    }

    public async Task UpdateRiderLocationAsync(
        int riderId,
        double latitude,
        double longitude,
        int status)
    {
        await ExecuteAsync(
            "sp_UpdateRiderLocation",
            new
            {
                RiderId = riderId,
                Latitude = latitude,
                Longitude = longitude,
                Status = status
            });
    }

    public async Task<List<NotificationDbo>> GetNotificationsAsync(int riderId)
    {
        return await QueryListAsync<NotificationDbo>(
            "sp_GetRiderNotifications",
            new
            {
                RiderId = riderId
            });
    }

    public async Task MarkNotificationReadAsync(
        int notificationId,
        int riderId)
    {
        await ExecuteAsync(
            "sp_MarkNotificationRead",
            new
            {
                NotificationId = notificationId,
                RiderId = riderId
            });
    }

    public async Task MarkAllNotificationsReadAsync(int riderId)
    {
        await ExecuteAsync(
            "sp_MarkAllNotificationsRead",
            new
            {
                RiderId = riderId
            });
    }
}