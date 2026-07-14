using AutoMapper;
using Module.Auth.Core.DBOs;
using Module.Auth.Core.DTOs;

namespace Shared.Infrastructure.Mapping;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<UserDbo, UserDto>();

        CreateMap<UserDbo, LoginResponseDto>();

        CreateMap<UserDbo, RegisterResponseDto>();
    }
}