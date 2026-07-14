using AutoMapper;
using Module.Contact.Core.DBOs;
using Module.Contact.Core.DTOs;

namespace Shared.Infrastructure.Mapping;

public class ContactProfile : Profile
{
    public ContactProfile()
    {
        CreateMap<ContactDbo, ContactDto>();
    }
}