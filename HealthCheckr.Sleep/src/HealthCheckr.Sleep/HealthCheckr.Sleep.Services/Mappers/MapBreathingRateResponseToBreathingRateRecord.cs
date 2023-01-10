using AutoMapper;
using HealthCheckr.Sleep.Common.Envelopes;
using HealthCheckr.Sleep.Common.FitbitResponses;

namespace HealthCheckr.Sleep.Services.Mappers
{
    public class MapBreathingRateResponseToBreathingRateRecord : Profile
    {
        public MapBreathingRateResponseToBreathingRateRecord()
        {
            CreateMap<BreathingRateResponseObject, BreathingRateRecord>()
                .ForMember(
                    dest => dest.Date,
                    opt => opt.MapFrom(src => src.br[0].dateTime))
                .ForMember(
                    dest => dest.BreathingRate,
                    opt => opt.MapFrom(src => src.br[0].value.breathingRate));
        }
    }
}
