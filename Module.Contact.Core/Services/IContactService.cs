using Module.Contact.Core.DTOs;
using Shared.Infrastructure.Models;

namespace Module.Contact.Core.Services;

public interface IContactService
{
    Task<ApiResponse<ContactResponseDto>> SendContactAsync(ContactRequestDto dto);
}
