using Microsoft.AspNetCore.Mvc;
using Module.Contact.Core.DTOs;
using Module.Contact.Core.Services;

namespace Module.Contact.Controllers;

[ApiController]
[Route("api/contact")]
public class ContactController : ControllerBase
{
    private readonly IContactService _service;

    public ContactController(IContactService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> SendContact([FromBody] ContactRequestDto dto)
        => Ok(await _service.SendContactAsync(dto));
}
