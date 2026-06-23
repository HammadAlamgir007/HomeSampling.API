using Microsoft.AspNetCore.Http;
using Module.Patient.Core.DTOs;
using Shared.Infrastructure.Models;

namespace Module.Patient.Core.Services;

public interface IPatientService
{
    Task<ApiResponse<List<TestDto>>> GetTestsAsync();
    Task<ApiResponse<BookingCreatedDto>> CreateBookingAsync(int userId, BookingRequestDto dto);
    Task<ApiResponse<List<BookingDto>>> GetBookingsAsync(int userId);
    Task<ApiResponse<bool>> CancelBookingAsync(int userId, int appointmentId);
    Task<(Stream stream, string contentType, string fileName)> DownloadReportAsync(int userId, string fileName);
}
