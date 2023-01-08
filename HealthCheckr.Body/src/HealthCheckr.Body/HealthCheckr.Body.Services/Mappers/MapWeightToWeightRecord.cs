using AutoMapper;
using HealthCheckr.Body.Common.Envelopes;
using HealthCheckr.Body.Common.FitbitResponses;

namespace HealthCheckr.Body.Services.Mappers
{
    public class MapWeightToWeightRecord : Profile
    {
        public MapWeightToWeightRecord()
        {
            CreateMap<Weight, WeightRecord>()
                .ForMember(
                    dest => dest.BmiValue,
                    opt => opt.MapFrom(src => src.bmi))
                .ForMember(
                    dest => dest.Date,
                    opt => opt.MapFrom(src => src.date))
                .ForMember(
                    dest => dest.FatPercentage,
                    opt => opt.MapFrom(src => src.fat))
                .ForMember(
                    dest => dest.Source,
                    opt => opt.MapFrom(src => src.source))
                .ForMember(
                    dest => dest.Time,
                    opt => opt.MapFrom(src => src.time))
                .ForMember(
                    dest => dest.WeightInKG,
                    opt => opt.MapFrom(src => src.weight));
        }
    }
}
