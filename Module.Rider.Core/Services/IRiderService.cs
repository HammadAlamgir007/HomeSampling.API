using Module.Rider.Core.DTOs;
using Shared.Infrastructure.Models;

namespace Module.Rider.Core.Services;

public interface IRiderService
{
    Task<ApiResponse<RiderTokenDto>> LoginAsync(RiderLoginDto dto);
    Task<ApiResponse<RiderProfileDto>> GetProfileAsync(int riderId);
    Task<ApiResponse<RiderMessageDto>> UpdateLocationAsync(int riderId, UpdateLocationDto dto);
    Task<ApiResponse<List<TaskDto>>> GetActiveTasksAsync(int riderId);
    Task<ApiResponse<List<TaskDto>>> GetTaskHistoryAsync(int riderId);
    Task<ApiResponse<RiderMessageDto>> UpdateTaskStatusAsync(int riderId, int appointmentId, string action, TaskActionDto dto);
    Task<ApiResponse<List<NotificationDto>>> GetNotificationsAsync(int riderId);
    Task<ApiResponse<RiderMessageDto>> MarkNotificationReadAsync(int riderId, int notificationId);
    Task<ApiResponse<RiderMessageDto>> MarkAllNotificationsReadAsync(int riderId);
}
