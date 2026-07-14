using Module.Admin.Core.DTOs;
using Module.Admin.Core.DBContext;
using Shared.Infrastructure.Enums;
using Shared.Infrastructure.Helpers;
using Shared.Infrastructure.Models;
using Shared.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using AutoMapper;
namespace Module.Admin.Core.Services;

public class AdminService : IAdminService
{
    private readonly IAdminDBContext _db;
    private readonly IFileService _file;
    private readonly IGuidService _guid;
    private readonly IMapper _mapper;

    public AdminService(IAdminDBContext db, IFileService file, IGuidService guid, IMapper mapper)
    {
        _db   = db;
        _file = file;
        _guid = guid;
        _mapper = mapper;
    }

    public async Task<ApiResponse<StatsDto>> GetDashboardStatsAsync()
    {
        var stats = await _db.GetDashboardStatsAsync();
        var result = _mapper.Map<StatsDto>(stats);
        return result.ToSuccessResponse(_guid.NewGuid());
    }

    public async Task<ApiResponse<AppointmentPagedDto>> GetAppointmentsAsync(
        int page, int perPage, string? search)
    {
        var rows = await _db.GetAppointmentsAsync(page, perPage, search);
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var result = new AppointmentPagedDto
        {
            TotalCount = rows.FirstOrDefault()?.TotalCount ?? 0,
            Page = page,
            PerPage = perPage,
            Items = _mapper.Map<List<AppointmentAdminDto>>(rows)
        };

        return result.ToSuccessResponse(_guid.NewGuid());
    }

    public async Task<ApiResponse<MessageDto>> UpdateStatusAsync(
        int appointmentId, int status)
    {
        await _db.UpdateAppointmentStatusAsync(appointmentId, status);
        var result = new MessageDto { Message = "Status updated successfully." };
        return result.ToSuccessResponse(_guid.NewGuid());
    }

    public async Task<ApiResponse<MessageDto>> BulkUpdateStatusAsync(BulkUpdateStatusDto dto)
    {
        await _db.BulkUpdateStatusAsync(dto.AppointmentIds, dto.Status);
        var result = new MessageDto { Message = $"{dto.AppointmentIds.Count} appointments updated." };
        return result.ToSuccessResponse(_guid.NewGuid());
    }

    public async Task<ApiResponse<MessageDto>> AssignRiderAsync(
        int appointmentId, AssignRiderDto dto)
    {
        await _db.AssignRiderAsync(appointmentId, dto.RiderId);
        var result = new MessageDto { Message = "Rider assigned successfully." };
        return result.ToSuccessResponse(_guid.NewGuid());
    }

    public async Task<ApiResponse<MessageDto>> AutoAssignRiderAsync(int appointmentId)
    {
        var traceId = _guid.NewGuid();
        var riderId = await _db.AutoAssignRiderAsync(appointmentId);
        if (riderId is null)
        {
            var err = new MessageDto { Message = "No available riders found." };
            return err.ToErrorResponse(traceId, "No available riders found.", "06");
        }
        var result = new MessageDto { Message = $"Rider {riderId} auto-assigned." };
        return result.ToSuccessResponse(traceId);
    }

    public async Task<ApiResponse<MessageDto>> UploadReportAsync(
        int appointmentId, IFormFile file)
    {
        var traceId = _guid.NewGuid();
        if (file.Length == 0)
        {
            var err = new MessageDto { Message = "File is empty." };
            return err.ToErrorResponse(traceId, "File is empty.", "07");
        }

        var relativePath = await _file.SaveAsync(file, "reports");
        var fileName = Path.GetFileName(relativePath);
        await _db.SaveReportPathAsync(appointmentId, fileName);

        var result = new MessageDto { Message = "Report uploaded successfully." };
        return result.ToSuccessResponse(traceId);
    }

    public async Task<ApiResponse<List<PatientAdminDto>>> GetAllPatientsAsync()
    {
        var patients = await _db.GetAllPatientsAsync();
        var result = _mapper.Map<List<PatientAdminDto>>(patients);
        return result.ToSuccessResponse(_guid.NewGuid());
    }

    public async Task<ApiResponse<List<TestAdminDto>>> GetAllTestsAsync()
    {
        var tests = await _db.GetAllTestsAsync();
        var result = _mapper.Map<List<TestAdminDto>>(tests);
        return result.ToSuccessResponse(_guid.NewGuid());
    }

    public async Task<ApiResponse<MessageDto>> CreateTestAsync(CreateTestDto dto)
    {
        await _db.CreateTestAsync(dto.Name, dto.Description, dto.Price, dto.Duration);
        var result = new MessageDto { Message = "Test created successfully." };
        return result.ToSuccessResponse(_guid.NewGuid());
    }

    public async Task<ApiResponse<MessageDto>> UpdateTestAsync(int testId, CreateTestDto dto)
    {
        await _db.UpdateTestAsync(testId, dto.Name, dto.Description, dto.Price, dto.Duration);
        var result = new MessageDto { Message = "Test updated successfully." };
        return result.ToSuccessResponse(_guid.NewGuid());
    }

    public async Task<ApiResponse<MessageDto>> DeleteTestAsync(int testId)
    {
        await _db.DeleteTestAsync(testId);
        var result = new MessageDto { Message = "Test deleted successfully." };
        return result.ToSuccessResponse(_guid.NewGuid());
    }

    public async Task<ApiResponse<List<RiderAdminDto>>> GetAllRidersAsync()
    {
        var riders = await _db.GetAllRidersAsync();
        var result = _mapper.Map<List<RiderAdminDto>>(riders);
        return result.ToSuccessResponse(_guid.NewGuid());
    }

    public async Task<ApiResponse<MessageDto>> CreateRiderAsync(CreateRiderDto dto)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        await _db.CreateRiderAsync(dto.Name, dto.Email, dto.Phone, passwordHash);
        var result = new MessageDto { Message = "Rider created successfully." };
        return result.ToSuccessResponse(_guid.NewGuid());
    }

    public async Task<ApiResponse<MessageDto>> DeleteRiderAsync(int riderId)
    {
        await _db.DeleteRiderAsync(riderId);
        var result = new MessageDto { Message = "Rider deleted successfully." };
        return result.ToSuccessResponse(_guid.NewGuid());
    }
}
