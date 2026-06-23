using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Module.Admin.Core.DTOs;
using Module.Admin.Core.Services;

namespace Module.Admin.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _service;

    public AdminController(IAdminService service)
    {
        _service = service;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
        => Ok(await _service.GetDashboardStatsAsync());

    [HttpGet("appointments")]
    public async Task<IActionResult> GetAppointments(
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        [FromQuery] string? search = null)
        => Ok(await _service.GetAppointmentsAsync(page, perPage, search));

    [HttpPut("appointments/{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
        => Ok(await _service.UpdateStatusAsync(id, dto.Status));

    [HttpPut("appointments/bulk-status")]
    public async Task<IActionResult> BulkUpdateStatus([FromBody] BulkUpdateStatusDto dto)
        => Ok(await _service.BulkUpdateStatusAsync(dto));

    [HttpPost("appointments/{id}/assign-rider")]
    public async Task<IActionResult> AssignRider(int id, [FromBody] AssignRiderDto dto)
        => Ok(await _service.AssignRiderAsync(id, dto));

    [HttpPost("appointments/{id}/auto-assign-rider")]
    public async Task<IActionResult> AutoAssignRider(int id)
        => Ok(await _service.AutoAssignRiderAsync(id));

    [HttpPost("upload-report/{id}")]
    public async Task<IActionResult> UploadReport(int id, IFormFile file)
        => Ok(await _service.UploadReportAsync(id, file));

    [HttpGet("patients")]
    public async Task<IActionResult> GetPatients()
        => Ok(await _service.GetAllPatientsAsync());

    [HttpGet("tests")]
    public async Task<IActionResult> GetTests()
        => Ok(await _service.GetAllTestsAsync());

    [HttpPost("tests")]
    public async Task<IActionResult> CreateTest([FromBody] CreateTestDto dto)
        => Ok(await _service.CreateTestAsync(dto));

    [HttpPut("tests/{id}")]
    public async Task<IActionResult> UpdateTest(int id, [FromBody] CreateTestDto dto)
        => Ok(await _service.UpdateTestAsync(id, dto));

    [HttpDelete("tests/{id}")]
    public async Task<IActionResult> DeleteTest(int id)
        => Ok(await _service.DeleteTestAsync(id));

    [HttpGet("riders")]
    public async Task<IActionResult> GetRiders()
        => Ok(await _service.GetAllRidersAsync());

    [HttpPost("riders")]
    public async Task<IActionResult> CreateRider([FromBody] CreateRiderDto dto)
        => Ok(await _service.CreateRiderAsync(dto));

    [HttpDelete("riders/{id}")]
    public async Task<IActionResult> DeleteRider(int id)
        => Ok(await _service.DeleteRiderAsync(id));
}
