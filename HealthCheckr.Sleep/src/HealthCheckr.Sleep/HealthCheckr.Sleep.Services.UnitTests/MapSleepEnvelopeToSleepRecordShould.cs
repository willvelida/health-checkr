using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using HealthCheckr.Sleep.Common.Envelopes;
using HealthCheckr.Sleep.Services.Mappers;

namespace HealthCheckr.Sleep.Services.UnitTests
{
    public class MapSleepEnvelopeToSleepRecordShould
    {
        private readonly IMapper _mapper;

        public MapSleepEnvelopeToSleepRecordShould()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapSleepEnvelopeToSleepRecord());
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public void MapResponseToRecord()
        {
            // ARRANGE
            var fixture = new Fixture();
            var sleepEnvelope = fixture.Create<SleepEnvelope>();
            sleepEnvelope.Id = "1";

            // ACT
            var expectedSleepRecord = _mapper.Map<SleepRecord>(sleepEnvelope);

            // ASSERT
            using (new AssertionScope())
            {
                expectedSleepRecord.Date.Should().Be(sleepEnvelope.Date);
                expectedSleepRecord.Efficiency.Should().Be(sleepEnvelope.Sleep.sleep[0].efficiency);
                expectedSleepRecord.Duration.Should().Be(sleepEnvelope.Sleep.sleep[0].duration);
                expectedSleepRecord.StartTime.Should().Be(sleepEnvelope.Sleep.sleep[0].startTime.ToString());
                expectedSleepRecord.EndTime.Should().Be(sleepEnvelope.Sleep.sleep[0].endTime.ToString());
                expectedSleepRecord.MinutesAfterWakeup.Should().Be(sleepEnvelope.Sleep.sleep[0].minutesAfterWakeup);
                expectedSleepRecord.MinutesAsleep.Should().Be(sleepEnvelope.Sleep.sleep[0].minutesAsleep);
                expectedSleepRecord.MinutesAwake.Should().Be(sleepEnvelope.Sleep.sleep[0].minutesAwake);
                expectedSleepRecord.MinutesToFallAsleep.Should().Be(sleepEnvelope.Sleep.sleep[0].minutesToFallAsleep);
                expectedSleepRecord.RestlessCount.Should().Be(sleepEnvelope.Sleep.sleep[0].restlessCount);
                expectedSleepRecord.RestlessDuration.Should().Be(sleepEnvelope.Sleep.sleep[0].restlessDuration);
                expectedSleepRecord.TimeInBed.Should().Be(sleepEnvelope.Sleep.sleep[0].timeInBed);
            }
        }
    }
}
