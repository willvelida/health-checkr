using AutoMapper;
using HealthCheckr.Sleep.Common.Envelopes;
using HealthCheckr.Sleep.Common.FitbitResponses;

namespace HealthCheckr.Sleep.Services.Mappers
{
    public class MapSleepEnvelopeToSleepRecord : Profile
    {
        public MapSleepEnvelopeToSleepRecord()
        {
            CreateMap<SleepResponseObject, SleepRecord>()
                .ForMember(
                    dest => dest.Date,
                    opt => opt.MapFrom(src => src.sleep[0].dateOfSleep))
                .ForMember(
                    dest => dest.Efficiency,
                    opt => opt.MapFrom(src => src.sleep[0].efficiency))
                .ForMember(
                    dest => dest.Duration,
                    opt => opt.MapFrom(src => src.sleep[0].duration))
                .ForMember(
                    dest => dest.StartTime,
                    opt => opt.MapFrom(src => src.sleep[0].startTime))
                .ForMember(
                    dest => dest.EndTime,
                    opt => opt.MapFrom(src => src.sleep[0].endTime))
                .ForMember(
                    dest => dest.MinutesAfterWakeup,
                    opt => opt.MapFrom(src => src.sleep[0].minutesAfterWakeup))
                .ForMember(
                    dest => dest.MinutesAsleep,
                    opt => opt.MapFrom(src => src.sleep[0].minutesAsleep))
                .ForMember(
                    dest => dest.MinutesAwake,
                    opt => opt.MapFrom(src => src.sleep[0].minutesAwake))
                .ForMember(
                    dest => dest.MinutesToFallAsleep,
                    opt => opt.MapFrom(src => src.sleep[0].minutesToFallAsleep))
                .ForMember(
                    dest => dest.RestlessCount,
                    opt => opt.MapFrom(src => src.sleep[0].restlessCount))
                .ForMember(
                    dest => dest.RestlessDuration,
                    opt => opt.MapFrom(src => src.sleep[0].restlessDuration))
                .ForMember(
                    dest => dest.TimeInBed,
                    opt => opt.MapFrom(src => src.sleep[0].timeInBed));
        }
    }
}
