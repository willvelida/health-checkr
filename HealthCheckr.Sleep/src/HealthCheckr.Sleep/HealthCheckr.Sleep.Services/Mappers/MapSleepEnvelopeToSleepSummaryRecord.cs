using AutoMapper;
using HealthCheckr.Sleep.Common.Envelopes;
using HealthCheckr.Sleep.Common.FitbitResponses;

namespace HealthCheckr.Sleep.Services.Mappers
{
    public class MapSleepEnvelopeToSleepSummaryRecord : Profile
    {
        public MapSleepEnvelopeToSleepSummaryRecord()
        {
            CreateMap<SleepResponseObject, SleepSummaryRecord>()
                .ForMember(
                    dest => dest.DeepSleep,
                    opt => opt.MapFrom(src => src.summary.stages.deep))
                .ForMember(
                    dest => dest.LightSleep,
                    opt => opt.MapFrom(src => src.summary.stages.light))
                .ForMember(
                    dest => dest.REMSleep,
                    opt => opt.MapFrom(src => src.summary.stages.rem))
                .ForMember(
                    dest => dest.AwakeMinutes,
                    opt => opt.MapFrom(src => src.summary.stages.wake))
                .ForMember(
                    dest => dest.TotalMinutesAsleep,
                    opt => opt.MapFrom(src => src.summary.totalMinutesAsleep))
                .ForMember(
                    dest => dest.TotalSleepRecords,
                    opt => opt.MapFrom(src => src.summary.totalSleepRecords))
                .ForMember(
                    dest => dest.TotalTimeInBed,
                    opt => opt.MapFrom(src => src.summary.totalTimeInBed))
                .ForMember(
                    dest => dest.Date,
                    opt => opt.MapFrom(src => src.sleep[0].dateOfSleep));
        }
    }
}
