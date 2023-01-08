using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using HealthCheckr.Body.Common.Envelopes;
using HealthCheckr.Body.Common.FitbitResponses;
using HealthCheckr.Body.Services.Mappers;

namespace HealthCheckr.Body.Services.UnitTests
{
    public class MapWeightToWeightRecordShould
    {
        private readonly IMapper _mapper;

        public MapWeightToWeightRecordShould()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapWeightToWeightRecord());
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public void MapResponseToRecord()
        {
            // ARRANGE
            var fixture = new Fixture();
            var weightResponse = fixture.Create<Weight>();

            // ACT
            var expectedWeightRecord = _mapper.Map<WeightRecord>(weightResponse);

            // ASSERT
            using (new AssertionScope())
            {
                expectedWeightRecord.BmiValue.Should().Be(weightResponse.bmi);
                expectedWeightRecord.Date.Should().Be(weightResponse.date);
                expectedWeightRecord.FatPercentage.Should().Be(weightResponse.fat);
                expectedWeightRecord.Source.Should().Be(weightResponse.source);
                expectedWeightRecord.Time.Should().Be(weightResponse.time);
                expectedWeightRecord.WeightInKG.Should().Be(weightResponse.weight);
            }
        }
    }
}
