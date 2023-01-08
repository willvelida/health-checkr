using AutoMapper;
using HealthCheckr.Body.Common.Envelopes;
using HealthCheckr.Body.Common.FitbitResponses;

namespace HealthCheckr.Body.Services.Mappers
{
    public class MapCardioResponseToV02Record : Profile
    {
        public MapCardioResponseToV02Record()
        {
            CreateMap<CardioResponseObject, V02Record>()
                .ForMember(
                    dest => dest.Date,
                    opt => opt.MapFrom(src => src.cardioScore[0].dateTime))
                .ForMember(
                    dest => dest.V02Max,
                    opt => opt.MapFrom(src => src.cardioScore[0].value.vo2Max));
        }
    }
}
