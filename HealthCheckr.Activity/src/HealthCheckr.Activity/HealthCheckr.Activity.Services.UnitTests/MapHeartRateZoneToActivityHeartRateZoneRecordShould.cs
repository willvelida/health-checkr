using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using HealthCheckr.Activity.Common.Envelopes;
using HealthCheckr.Activity.Common.FitbitResponses;
using HealthCheckr.Activity.Services.Mappers;

namespace HealthCheckr.Activity.Services.UnitTests
{
    public class MapHeartRateZoneToActivityHeartRateZoneRecordShould
    {
        private readonly IMapper _mapper;

        public MapHeartRateZoneToActivityHeartRateZoneRecordShould()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapHeartRateZoneToActivityHeartRateZonesRecord());
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public void MapResponseToRecord()
        {
            // ARRANGE
            var fixture = new Fixture();
            var heartRateZone = fixture.Create<HeartRateZone>();

            // ACT
            var expectedActivityHeartRateZoneRecord = _mapper.Map<ActivityHeartRateZonesRecord>(heartRateZone);

            // ASSERT
            using (new AssertionScope())
            {
                expectedActivityHeartRateZoneRecord.Name.Should().Be(heartRateZone.name);
                expectedActivityHeartRateZoneRecord.Minutes.Should().Be(heartRateZone.minutes);
                expectedActivityHeartRateZoneRecord.MaxHR.Should().Be(heartRateZone.max);
                expectedActivityHeartRateZoneRecord.MinHR.Should().Be(heartRateZone.min);
                expectedActivityHeartRateZoneRecord.CaloriesOut.Should().Be(heartRateZone.caloriesOut);
            }
        }
    }
}
