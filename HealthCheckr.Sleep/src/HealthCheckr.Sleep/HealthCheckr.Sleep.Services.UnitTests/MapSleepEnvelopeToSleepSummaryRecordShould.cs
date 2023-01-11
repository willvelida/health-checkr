using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using HealthCheckr.Sleep.Common.Envelopes;
using HealthCheckr.Sleep.Services.Mappers;

namespace HealthCheckr.Sleep.Services.UnitTests
{
    public class MapSleepEnvelopeToSleepSummaryRecordShould
    {
        private readonly IMapper _mapper;

        public MapSleepEnvelopeToSleepSummaryRecordShould()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapSleepEnvelopeToSleepSummaryRecord());
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
            var expectedSleepSummaryRecord = _mapper.Map<SleepSummaryRecord>(sleepEnvelope);

            // ASSERT
            using (new AssertionScope())
            {
                expectedSleepSummaryRecord.Date.Should().Be(sleepEnvelope.Date);
                expectedSleepSummaryRecord.DeepSleep.Should().Be(sleepEnvelope.Sleep.summary.stages.deep);
                expectedSleepSummaryRecord.LightSleep.Should().Be(sleepEnvelope.Sleep.summary.stages.light);
                expectedSleepSummaryRecord.REMSleep.Should().Be(sleepEnvelope.Sleep.summary.stages.rem);
                expectedSleepSummaryRecord.AwakeMinutes.Should().Be(sleepEnvelope.Sleep.summary.stages.wake);
                expectedSleepSummaryRecord.TotalMinutesAsleep.Should().Be(sleepEnvelope.Sleep.summary.totalMinutesAsleep);
                expectedSleepSummaryRecord.TotalSleepRecords.Should().Be(sleepEnvelope.Sleep.summary.totalSleepRecords);
                expectedSleepSummaryRecord.TotalTimeInBed.Should().Be(sleepEnvelope.Sleep.summary.totalTimeInBed);
            }
        }
    }
}
