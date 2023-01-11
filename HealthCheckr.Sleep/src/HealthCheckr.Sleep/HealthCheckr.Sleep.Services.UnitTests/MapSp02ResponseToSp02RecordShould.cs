using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using HealthCheckr.Sleep.Common.Envelopes;
using HealthCheckr.Sleep.Common.FitbitResponses;
using HealthCheckr.Sleep.Services.Mappers;

namespace HealthCheckr.Sleep.Services.UnitTests
{
    public class MapSp02ResponseToSp02RecordShould
    {
        private readonly IMapper _mapper;

        public MapSp02ResponseToSp02RecordShould()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapSp02ResponseToSp02Record());
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public void MapResponseToRecord()
        {
            // ARRANGE
            var fixture = new Fixture();
            var sp02Response = fixture.Create<Sp02ResponseObject>();

            // ACT
            var expectedSp02Record = _mapper.Map<Sp02Record>(sp02Response);

            // ASSERT
            using (new AssertionScope())
            {
                expectedSp02Record.MaxValue.Should().Be(sp02Response.value.max);
                expectedSp02Record.AvgValue.Should().Be(sp02Response.value.avg);
                expectedSp02Record.MinValue.Should().Be(sp02Response.value.min);
                expectedSp02Record.Date.Should().Be(sp02Response.dateTime);
            }
        }
    }
}
