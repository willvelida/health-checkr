using AutoMapper;
using HealthCheckr.Activity.Common.Envelopes;
using res = HealthCheckr.Activity.Common.FitbitResponses;

namespace HealthCheckr.Activity.Services.Mappers
{
    public class MapActivityToActivityRecord : Profile
    {
        public MapActivityToActivityRecord()
        {
            CreateMap<res.Activity, ActivityRecord>()
                .ForMember(dest => dest.ActivityName,
                    opt => opt.MapFrom(src => src.name))
                .ForMember(dest => dest.Calories,
                    opt => opt.MapFrom(src => src.calories))
                .ForMember(dest => dest.Duration,
                    opt => opt.MapFrom(src => src.duration))
                .ForMember(dest => dest.Date,
                    opt => opt.MapFrom(src => src.startDate))
                .ForMember(dest => dest.Time,
                    opt => opt.MapFrom(src => src.startTime))
                .ForMember(dest => dest.Steps,
                    opt => opt.MapFrom(src => src.steps));
        }
    }
}
