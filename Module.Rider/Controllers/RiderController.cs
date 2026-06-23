using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Module.Rider.Core.DTOs;
using Module.Rider.Core.Services;
using System.Security.Claims;

namespace Module.Rider.Controllers;

[ApiController]
[Route("api/rider")]
public class RiderController : ControllerBase
{
    private readonly IRiderService _service;

    public RiderController(IRiderService service)
    {
        _service = service;
    }

    private int GetRiderId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] RiderLoginDto dto)
        => Ok(await _service.LoginAsync(dto));

    [HttpGet("profile")]
    [Authorize(Roles = "Rider")]
    public async Task<IActionResult> GetProfile()
        => Ok(await _service.GetProfileAsync(GetRiderId()));

    [HttpPut("profile")]
    [Authorize(Roles = "Rider")]
    public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationDto dto)
        => Ok(await _service.UpdateLocationAsync(GetRiderId(), dto));

    [HttpGet("tasks")]
    [Authorize(Roles = "Rider")]
    public async Task<IActionResult> GetActiveTasks()
        => Ok(await _service.GetActiveTasksAsync(GetRiderId()));

    [HttpGet("tasks/history")]
    [Authorize(Roles = "Rider")]
    public async Task<IActionResult> GetTaskHistory()
        => Ok(await _service.GetTaskHistoryAsync(GetRiderId()));

    [HttpPut("tasks/{id}/on-way")]
    [Authorize(Roles = "Rider")]
    public async Task<IActionResult> OnWay(int id, [FromForm] TaskActionDto dto)
        => Ok(await _service.UpdateTaskStatusAsync(GetRiderId(), id, "OnWay", dto));

    [HttpPut("tasks/{id}/arrive")]
    [Authorize(Roles = "Rider")]
    public async Task<IActionResult> Arrive(int id, [FromForm] TaskActionDto dto)
        => Ok(await _service.UpdateTaskStatusAsync(GetRiderId(), id, "Arrived", dto));

    [HttpPut("tasks/{id}/collect")]
    [Authorize(Roles = "Rider")]
    public async Task<IActionResult> Collect(int id, [FromForm] TaskActionDto dto)
        => Ok(await _service.UpdateTaskStatusAsync(GetRiderId(), id, "Collected", dto));

    [HttpPut("tasks/{id}/deliver")]
    [Authorize(Roles = "Rider")]
    public async Task<IActionResult> Deliver(int id, [FromForm] TaskActionDto dto)
        => Ok(await _service.UpdateTaskStatusAsync(GetRiderId(), id, "Delivered", dto));

    [HttpGet("notifications")]
    [Authorize(Roles = "Rider")]
    public async Task<IActionResult> GetNotifications()
        => Ok(await _service.GetNotificationsAsync(GetRiderId()));

    [HttpPut("notifications/{id}/read")]
    [Authorize(Roles = "Rider")]
    public async Task<IActionResult> MarkRead(int id)
        => Ok(await _service.MarkNotificationReadAsync(GetRiderId(), id));

    [HttpPut("notifications/read-all")]
    [Authorize(Roles = "Rider")]
    public async Task<IActionResult> MarkAllRead()
        => Ok(await _service.MarkAllNotificationsReadAsync(GetRiderId()));
}
