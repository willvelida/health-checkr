using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using HealthCheckr.Sleep.Common.Envelopes;
using HealthCheckr.Sleep.Common.FitbitResponses;
using HealthCheckr.Sleep.Services.Mappers;

namespace HealthCheckr.Sleep.Services.UnitTests
{
    public class MapBreathingRateResponseToBreathingRateRecordShould
    {
        private readonly IMapper _mapper;

        public MapBreathingRateResponseToBreathingRateRecordShould()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapBreathingRateResponseToBreathingRateRecord());
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public void MapResponseToRecord()
        {
            // ARRANGE
            var fixture = new Fixture();
            var breathingResponse = fixture.Create<BreathingRateResponseObject>();

            // ACT
            var expectedBreathingRecord = _mapper.Map<BreathingRateRecord>(breathingResponse);

            // ASSERT
            using (new AssertionScope())
            {
                expectedBreathingRecord.BreathingRate.Should().Be(breathingResponse.br[0].value.breathingRate);
            }
        }
    }
}