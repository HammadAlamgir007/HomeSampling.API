using Module.Rider.Core.DBOs;

namespace Module.Rider.Core.DBContext;

public interface IRiderDBContext
{
    Task<RiderDbo?> GetRiderByEmailAsync(string email);
    Task<RiderDbo?> GetRiderByIdAsync(int riderId);
    Task<List<TaskDbo>> GetActiveTasksAsync(int riderId);
    Task<List<TaskDbo>> GetTaskHistoryAsync(int riderId);
    Task<TaskDbo?> GetTaskByIdAsync(int appointmentId, int riderId);
    Task UpdateAppointmentStatusAsync(int appointmentId, int status);
    Task InsertTaskLogAsync(int appointmentId, int riderId, string action,
        string? notes, string? photoPath, double? latitude, double? longitude);
    Task UpdateRiderLocationAsync(int riderId, double latitude,
        double longitude, int status);
    Task<List<NotificationDbo>> GetNotificationsAsync(int riderId);
    Task MarkNotificationReadAsync(int notificationId, int riderId);
    Task MarkAllNotificationsReadAsync(int riderId);
}
