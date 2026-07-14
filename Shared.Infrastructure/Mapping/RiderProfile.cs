using AutoMapper;
using Module.Rider.Core.DBOs;
using Module.Rider.Core.DTOs;
using Shared.Infrastructure.Enums;

namespace Module.Rider.Core.Mapping;

public class RiderProfile : Profile
{
    public RiderProfile()
    {
        CreateMap<RiderDbo, RiderProfileDto>()
            .ForMember(d => d.Status,
                o => o.MapFrom(s => ((RiderStatus)s.Status).ToString()));

        CreateMap<RiderDbo, RiderTokenDto>();

        CreateMap<TaskDbo, TaskDto>()
            .ForMember(d => d.Status,
                o => o.MapFrom(s => ((AppointmentStatus)s.Status).ToString()));

        CreateMap<NotificationDbo, NotificationDto>();
    }
}