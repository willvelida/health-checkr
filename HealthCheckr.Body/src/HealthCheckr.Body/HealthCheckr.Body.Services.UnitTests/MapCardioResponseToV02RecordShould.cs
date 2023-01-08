using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using HealthCheckr.Body.Common.Envelopes;
using HealthCheckr.Body.Common.FitbitResponses;
using HealthCheckr.Body.Services.Mappers;

namespace HealthCheckr.Body.Services.UnitTests
{
    public class MapCardioResponseToV02RecordShould
    {
        private readonly IMapper _mapper;

        public MapCardioResponseToV02RecordShould()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapCardioResponseToV02Record());
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public void MapResponseToRecord()
        {
            // ARRANGE
            var fixture = new Fixture();
            var cardioResponse = fixture.Create<CardioResponseObject>();

            // ACT
            var expectedV02Record = _mapper.Map<V02Record>(cardioResponse);

            // ASSERT
            using (new AssertionScope())
            {
                expectedV02Record.Date.Should().Be(cardioResponse.cardioScore[0].dateTime);
                expectedV02Record.V02Max.Should().Be(cardioResponse.cardioScore[0].value.vo2Max);
            }
        }
    }
}
