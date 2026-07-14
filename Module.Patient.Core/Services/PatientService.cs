using Module.Patient.Core.DTOs;
using Module.Patient.Core.DBContext;
using Shared.Infrastructure.Enums;
using Shared.Infrastructure.Exceptions;
using Shared.Infrastructure.Helpers;
using Shared.Infrastructure.Models;
using Shared.Infrastructure.Services;
using AutoMapper;
namespace Module.Patient.Core.Services;

public class PatientService : IPatientService
{
    private readonly IPatientDBContext _db;
    private readonly IEmailService _email;
    private readonly IFileService _file;
    private readonly IGuidService _guid;
    private readonly IMapper _mapper;

    public PatientService(
        IPatientDBContext db,
        IEmailService email,
        IFileService file,
        IGuidService guid,
        IMapper mapper)
    {
        _db    = db;
        _email = email;
        _file  = file;
        _guid  = guid;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<TestDto>>> GetTestsAsync()
    {
        var tests = await _db.GetActiveTestsAsync();
        var result = _mapper.Map<List < TestDto >> (tests);
        //    tests.Select(t => new TestDto
        //{
        //    TestId      = t.TestId,
        //    Name        = t.Name,
        //    Description = t.Description,
        //    Price       = t.Price,
        //    Duration    = t.Duration
        //}).ToList();

        return result.ToSuccessResponse(_guid.NewGuid());
    }

    public async Task<ApiResponse<BookingCreatedDto>> CreateBookingAsync(
        int userId, BookingRequestDto dto)
    {
        var traceId = _guid.NewGuid();

        // Check for duplicate booking on same day
        var existing = await _db.GetAppointmentByDateAsync(
            userId, dto.TestId, dto.ScheduledDate);

        if (existing is not null)
        {
            var err = new BookingCreatedDto();
            return err.ToErrorResponse(traceId,
                "You already have a booking for this test on this date.", "01");
        }

        var appointmentId = await _db.CreateBookingAsync(
            userId, dto.TestId, dto.ScheduledDate,
            dto.Address, dto.Latitude, dto.Longitude);

        var result = new BookingCreatedDto
        {
            AppointmentId = appointmentId,
            Message       = "Booking created successfully."
        };
        return result.ToSuccessResponse(traceId, "Booking created successfully.");
    }

    public async Task<ApiResponse<List<BookingDto>>> GetBookingsAsync(int userId)
    {
        var bookings = await _db.GetPatientBookingsAsync(userId);
        var result = _mapper.Map<List<BookingDto>>(bookings);
       

        return result.ToSuccessResponse(_guid.NewGuid());
    }

    public async Task<ApiResponse<bool>> CancelBookingAsync(
        int userId, int appointmentId)
    {
        var traceId = _guid.NewGuid();
        var booking = await _db.GetAppointmentByIdAsync(appointmentId, userId);

        if (booking is null)
            return false.ToErrorResponse(traceId, "Booking not found.", "04");

        if (booking.Status != (int)AppointmentStatus.Pending)
            return false.ToErrorResponse(traceId,
                "Only pending bookings can be cancelled.", "05");

        await _db.CancelBookingAsync(appointmentId);
        return true.ToSuccessResponse(traceId, "Booking cancelled successfully.");
    }

    public async Task<(Stream stream, string contentType, string fileName)>
        DownloadReportAsync(int userId, string fileName)
    {
        // Verify ownership — patient can only download their own report
        var appointment = await _db.GetAppointmentByReportAsync(fileName, userId);
        if (appointment is null)
            throw new NotFoundException("Report not found or access denied.");

        var relativePath = $"uploads/reports/{fileName}";
        if (!_file.Exists(relativePath))
            throw new NotFoundException("Report file not found on server.");

        var (stream, contentType) = await _file.GetAsync(relativePath);
        return (stream, contentType, fileName);
    }
}
