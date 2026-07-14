using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Module.Rider.Core.DTOs;
using Module.Rider.Core.DBContext;
using Shared.Infrastructure.Enums;
using Shared.Infrastructure.Helpers;
using Shared.Infrastructure.Models;
using Shared.Infrastructure.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Module.Rider.Core.DBOs;
using AutoMapper;

namespace Module.Rider.Core.Services;

public class RiderService : IRiderService
{
    private readonly IRiderDBContext _db;
    private readonly IFileService _file;
    private readonly IGuidService _guid;
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;
    private static readonly Dictionary<AppointmentStatus, AppointmentStatus> ValidTransitions = new()
    {
        [AppointmentStatus.AssignedRider] = AppointmentStatus.OnWay,
        [AppointmentStatus.OnWay]         = AppointmentStatus.Arrived,
        [AppointmentStatus.Arrived]       = AppointmentStatus.Collected,
        [AppointmentStatus.Collected]     = AppointmentStatus.Delivered
    };

    public RiderService(
     IRiderDBContext db,
     IFileService file,
     IGuidService guid,
     IConfiguration config,
     IMapper mapper)
    {
        _db = db;
        _file = file;
        _guid = guid;
        _config = config;
        _mapper = mapper;
    }
    public async Task<ApiResponse<RiderTokenDto>> LoginAsync(RiderLoginDto dto)
    {
        var traceId = _guid.NewGuid();
        var rider   = await _db.GetRiderByEmailAsync(dto.Email);

        if (rider is null || !BCrypt.Net.BCrypt.Verify(dto.Password, rider.PasswordHash))
        {
            var err = new RiderTokenDto();
            return err.ToErrorResponse(traceId, "Invalid email or password.", "01");
        }

        var token  = GenerateJwtToken(rider.RiderId, rider.Email, rider.Name);
        var result = _mapper.Map<RiderTokenDto>(rider);
        result.Token = token;
        return result.ToSuccessResponse(traceId, "Login successful.");
    }

    public async Task<ApiResponse<RiderProfileDto>> GetProfileAsync(int riderId)
    {
        var rider = await _db.GetRiderByIdAsync(riderId);
        if (rider is null)
            return new RiderProfileDto().ToErrorResponse(_guid.NewGuid(), "Rider not found.", "04");

        var result = _mapper.Map<RiderProfileDto>(rider);
        return result.ToSuccessResponse(_guid.NewGuid());
    }

    public async Task<ApiResponse<RiderMessageDto>> UpdateLocationAsync(
        int riderId, UpdateLocationDto dto)
    {
        await _db.UpdateRiderLocationAsync(riderId, dto.Latitude, dto.Longitude, dto.Status);
        var result = new RiderMessageDto { Message = "Location updated." };
        return result.ToSuccessResponse(_guid.NewGuid());
    }

    public async Task<ApiResponse<List<TaskDto>>> GetActiveTasksAsync(int riderId)
    {
        var tasks  = await _db.GetActiveTasksAsync(riderId);
        var result = _mapper.Map<List<TaskDto>>(tasks);
        return result.ToSuccessResponse(_guid.NewGuid());
    }

    public async Task<ApiResponse<List<TaskDto>>> GetTaskHistoryAsync(int riderId)
    {
        var tasks  = await _db.GetTaskHistoryAsync(riderId);
        var result = _mapper.Map<List<TaskDto>>(tasks);
        return result.ToSuccessResponse(_guid.NewGuid());
    }

    public async Task<ApiResponse<RiderMessageDto>> UpdateTaskStatusAsync(
        int riderId, int appointmentId, string action, TaskActionDto dto)
    {
        var traceId = _guid.NewGuid();
        var task    = await _db.GetTaskByIdAsync(appointmentId, riderId);

        if (task is null)
            return new RiderMessageDto().ToErrorResponse(traceId, "Task not found.", "04");

        var currentStatus = (AppointmentStatus)task.Status;

        // Validate the action matches a valid transition
        if (!ValidTransitions.TryGetValue(currentStatus, out var nextStatus)
            || nextStatus.ToString() != action)
        {
            return new RiderMessageDto().ToErrorResponse(traceId,
                $"Cannot perform '{action}' from current status '{currentStatus}'.", "05");
        }

        // Save sample photo if provided
        string? photoPath = null;
        if (dto.Photo is not null)
            photoPath = await _file.SaveAsync(dto.Photo, "samples");

        // Insert immutable task log entry
        await _db.InsertTaskLogAsync(appointmentId, riderId,
            action, dto.Notes, photoPath, dto.Latitude, dto.Longitude);

        // Update appointment status
        await _db.UpdateAppointmentStatusAsync(appointmentId, (int)nextStatus);

        var result = new RiderMessageDto
        {
            Message = $"Task status updated to {nextStatus}."
        };
        return result.ToSuccessResponse(traceId);
    }

    public async Task<ApiResponse<List<NotificationDto>>> GetNotificationsAsync(int riderId)
    {
        var notifications = await _db.GetNotificationsAsync(riderId);
        var result = _mapper.Map<List<NotificationDto>>(notifications);
        return result.ToSuccessResponse(_guid.NewGuid());
    }

    public async Task<ApiResponse<RiderMessageDto>> MarkNotificationReadAsync(
        int riderId, int notificationId)
    {
        await _db.MarkNotificationReadAsync(notificationId, riderId);
        var result = new RiderMessageDto { Message = "Notification marked as read." };
        return result.ToSuccessResponse(_guid.NewGuid());
    }

    public async Task<ApiResponse<RiderMessageDto>> MarkAllNotificationsReadAsync(int riderId)
    {
        await _db.MarkAllNotificationsReadAsync(riderId);
        var result = new RiderMessageDto { Message = "All notifications marked as read." };
        return result.ToSuccessResponse(_guid.NewGuid());
    }

   
    private string GenerateJwtToken(int riderId, string email, string name)
    {
        var key    = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds  = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, riderId.ToString()),
            new Claim(ClaimTypes.Email,          email),
            new Claim(ClaimTypes.Name,           name),
            new Claim(ClaimTypes.Role,           "Rider")
        };
        var token = new JwtSecurityToken(
            issuer:             _config["Jwt:Issuer"],
            audience:           _config["Jwt:Audience"],
            claims:             claims,
            expires:            DateTime.UtcNow.AddHours(12),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
