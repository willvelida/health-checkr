using AutoMapper;
using HealthCheckr.Sleep.Common.Envelopes;

namespace HealthCheckr.Sleep.Services.Mappers
{
    public class MapSleepEnvelopeToSleepSummaryRecord : Profile
    {
        public MapSleepEnvelopeToSleepSummaryRecord()
        {
            CreateMap<SleepEnvelope, SleepSummaryRecord>()
                .ForMember(
                    dest => dest.DeepSleep,
                    opt => opt.MapFrom(src => src.Sleep.summary.stages.deep))
                .ForMember(
                    dest => dest.LightSleep,
                    opt => opt.MapFrom(src => src.Sleep.summary.stages.light))
                .ForMember(
                    dest => dest.REMSleep,
                    opt => opt.MapFrom(src => src.Sleep.summary.stages.rem))
                .ForMember(
                    dest => dest.AwakeMinutes,
                    opt => opt.MapFrom(src => src.Sleep.summary.stages.wake))
                .ForMember(
                    dest => dest.TotalMinutesAsleep,
                    opt => opt.MapFrom(src => src.Sleep.summary.totalMinutesAsleep))
                .ForMember(
                    dest => dest.TotalSleepRecords,
                    opt => opt.MapFrom(src => src.Sleep.summary.totalSleepRecords))
                .ForMember(
                    dest => dest.TotalTimeInBed,
                    opt => opt.MapFrom(src => src.Sleep.summary.totalTimeInBed))
                .ForMember(
                    dest => dest.Date,
                    opt => opt.MapFrom(src => src.Date));
        }
    }
}
