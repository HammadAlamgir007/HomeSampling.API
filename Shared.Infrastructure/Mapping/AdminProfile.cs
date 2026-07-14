using AutoMapper;
using Module.Admin.Core.DBOs;
using Module.Admin.Core.DTOs;
using Shared.Infrastructure.Enums;

namespace Shared.Infrastructure.Mapping;

public class AdminProfile : Profile
{
    public AdminProfile()
    {
        CreateMap<StatsDbo, StatsDto>();

        CreateMap<PatientAdminDbo, PatientAdminDto>();

        CreateMap<TestAdminDbo, TestAdminDto>();

        CreateMap<AppointmentAdminDbo, AppointmentAdminDto>()
            .ForMember(
                d => d.Status,
                o => o.MapFrom(s => ((AppointmentStatus)s.Status).ToString()));

        CreateMap<RiderAdminDbo, RiderAdminDto>()
            .ForMember(
                d => d.Status,
                o => o.MapFrom(s => ((RiderStatus)s.Status).ToString()));
    }
}