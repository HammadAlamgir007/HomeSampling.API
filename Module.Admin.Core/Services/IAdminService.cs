using Microsoft.AspNetCore.Http;
using Module.Admin.Core.DTOs;
using Shared.Infrastructure.Models;

namespace Module.Admin.Core.Services;

public interface IAdminService
{
    Task<ApiResponse<StatsDto>> GetDashboardStatsAsync();
    Task<ApiResponse<AppointmentPagedDto>> GetAppointmentsAsync(int page, int perPage, string? search);
    Task<ApiResponse<MessageDto>> UpdateStatusAsync(int appointmentId, int status);
    Task<ApiResponse<MessageDto>> BulkUpdateStatusAsync(BulkUpdateStatusDto dto);
    Task<ApiResponse<MessageDto>> AssignRiderAsync(int appointmentId, AssignRiderDto dto);
    Task<ApiResponse<MessageDto>> AutoAssignRiderAsync(int appointmentId);
    Task<ApiResponse<MessageDto>> UploadReportAsync(int appointmentId, IFormFile file);
    Task<ApiResponse<List<PatientAdminDto>>> GetAllPatientsAsync();
    Task<ApiResponse<List<TestAdminDto>>> GetAllTestsAsync();
    Task<ApiResponse<MessageDto>> CreateTestAsync(CreateTestDto dto);
    Task<ApiResponse<MessageDto>> UpdateTestAsync(int testId, CreateTestDto dto);
    Task<ApiResponse<MessageDto>> DeleteTestAsync(int testId);
    Task<ApiResponse<List<RiderAdminDto>>> GetAllRidersAsync();
    Task<ApiResponse<MessageDto>> CreateRiderAsync(CreateRiderDto dto);
    Task<ApiResponse<MessageDto>> DeleteRiderAsync(int riderId);
}
