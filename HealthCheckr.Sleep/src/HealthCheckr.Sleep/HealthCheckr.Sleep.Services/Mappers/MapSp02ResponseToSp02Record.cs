using AutoMapper;
using HealthCheckr.Sleep.Common.Envelopes;
using HealthCheckr.Sleep.Common.FitbitResponses;

namespace HealthCheckr.Sleep.Services.Mappers
{
    public class MapSp02ResponseToSp02Record : Profile
    {
        public MapSp02ResponseToSp02Record()
        {
            CreateMap<Sp02ResponseObject, Sp02Record>()
                .ForMember(
                    dest => dest.Date,
                    opt => opt.MapFrom(src => src.dateTime))
                .ForMember(
                    dest => dest.MinValue,
                    opt => opt.MapFrom(src => src.value.min))
                .ForMember(
                    dest => dest.MaxValue,
                    opt => opt.MapFrom(src => src.value.max))
                .ForMember(
                    dest => dest.AvgValue,
                    opt => opt.MapFrom(src => src.value.avg));
        }
    }
}
