using AutoMapper;
using HealthCheckr.Activity.Common.Envelopes;
using HealthCheckr.Activity.Common.FitbitResponses;

namespace HealthCheckr.Activity.Services.Mappers
{
    public class MapDistanceToActivityDistancesRecord : Profile
    {
        public MapDistanceToActivityDistancesRecord()
        {
            CreateMap<Distance, ActivityDistancesRecord>()
                .ForMember(dest => dest.ActivityType,
                    opt => opt.MapFrom(src => src.activity))
                .ForMember(dest => dest.Distance,
                    opt => opt.MapFrom(src => Math.Round(src.distance, 2)));
        }
    }
}
