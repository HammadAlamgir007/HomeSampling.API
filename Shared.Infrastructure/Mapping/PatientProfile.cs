using AutoMapper;
using Module.Patient.Core.DBOs;
using Module.Patient.Core.DTOs;
using Shared.Infrastructure.Enums;

namespace Shared.Infrastructure.Mapping;

public class PatientProfile : Profile
{
    public PatientProfile()
    {
        CreateMap<TestDbo, TestDto>();

        CreateMap<AppointmentDbo, BookingDto>()
            .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src =>
                    ((AppointmentStatus)src.Status).ToString()));
    }
}