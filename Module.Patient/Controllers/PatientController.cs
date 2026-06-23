using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Module.Patient.Core.DTOs;
using Module.Patient.Core.Services;
using System.Security.Claims;

namespace Module.Patient.Controllers;

[ApiController]
[Route("api/patient")]
public class PatientController : ControllerBase
{
    private readonly IPatientService _service;

    public PatientController(IPatientService service)
    {
        _service = service;
    }
   
    private int GetUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpGet("tests")]
    public async Task<IActionResult> GetTests()
    {
        var response = await _service.GetTestsAsync();
        return Ok(response);
    }

    [HttpPost("book")]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> CreateBooking([FromBody] BookingRequestDto dto)
    {
        var response = await _service.CreateBookingAsync(GetUserId(), dto);
        return Ok(response);
    }

    [HttpGet("bookings")]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> GetBookings()
    {
        var response = await _service.GetBookingsAsync(GetUserId());
        return Ok(response);
    }

    [HttpDelete("bookings/{appointmentId}")]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> CancelBooking(int appointmentId)
    {
        var response = await _service.CancelBookingAsync(GetUserId(), appointmentId);
        return Ok(response);
    }

    [HttpGet("reports/{fileName}")]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> DownloadReport(string fileName)
    {
        var (stream, contentType, name) =
            await _service.DownloadReportAsync(GetUserId(), fileName);
        return File(stream, contentType, name);
    }
}
