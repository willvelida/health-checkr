using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using HealthCheckr.Sleep.Common.Envelopes;
using HealthCheckr.Sleep.Common.FitbitResponses;
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
            var sleepResponseObject = fixture.Create<SleepResponseObject>();

            // ACT
            var expectedSleepSummaryRecord = _mapper.Map<SleepSummaryRecord>(sleepResponseObject);

            // ASSERT
            using (new AssertionScope())
            {
                expectedSleepSummaryRecord.Date.Should().Be(sleepResponseObject.sleep[0].dateOfSleep);
                expectedSleepSummaryRecord.DeepSleep.Should().Be(sleepResponseObject.summary.stages.deep);
                expectedSleepSummaryRecord.LightSleep.Should().Be(sleepResponseObject.summary.stages.light);
                expectedSleepSummaryRecord.REMSleep.Should().Be(sleepResponseObject.summary.stages.rem);
                expectedSleepSummaryRecord.AwakeMinutes.Should().Be(sleepResponseObject.summary.stages.wake);
                expectedSleepSummaryRecord.TotalMinutesAsleep.Should().Be(sleepResponseObject.summary.totalMinutesAsleep);
                expectedSleepSummaryRecord.TotalSleepRecords.Should().Be(sleepResponseObject.summary.totalSleepRecords);
                expectedSleepSummaryRecord.TotalTimeInBed.Should().Be(sleepResponseObject.summary.totalTimeInBed);
            }
        }
    }
}
