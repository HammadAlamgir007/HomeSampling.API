using Module.Contact.Core.DTOs;
using Shared.Infrastructure.Helpers;
using Shared.Infrastructure.Models;
using Shared.Infrastructure.Services;
using AutoMapper;
namespace Module.Contact.Core.Services;

public class ContactService : IContactService
{
    private readonly IEmailService _email;
    private readonly IGuidService _guid;
    private readonly IMapper _mapper;

    public ContactService(IEmailService email, IGuidService guid , IMapper mapper)
    {
        _email = email;
        _guid  = guid;
        _mapper= mapper;
    }

    public async Task<ApiResponse<ContactResponseDto>> SendContactAsync(ContactRequestDto dto)
    {
        await _email.SendContactEmailAsync(dto.Name, dto.Email, dto.Message);
        var result = new ContactResponseDto
        {
            Message = "Your message has been sent. We will get back to you soon."
        };
        return result.ToSuccessResponse(_guid.NewGuid());
    }
}
