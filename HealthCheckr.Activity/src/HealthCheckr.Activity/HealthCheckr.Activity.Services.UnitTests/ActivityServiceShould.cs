using AutoFixture;
using AutoMapper;
using Azure.Messaging.ServiceBus;
using FluentAssertions;
using HealthCheckr.Activity.Common.Envelopes;
using HealthCheckr.Activity.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using res = HealthCheckr.Activity.Common.FitbitResponses;

namespace HealthCheckr.Activity.Services.UnitTests
{
    public class ActivityServiceShould
    {
        private Mock<ServiceBusClient> _serviceBusMock;
        private Mock<ServiceBusSender> _serviceBusSenderMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IActivityRepository> _activityRepoMock;
        private Mock<ICosmosDbRepository> _cosmosRepoMock;
        private Mock<ILogger<ActivityService>> _loggerMock;

        private ActivityService _sut;

        public ActivityServiceShould()
        {
            _serviceBusMock = new Mock<ServiceBusClient>();
            _serviceBusSenderMock = new Mock<ServiceBusSender>();
            _cosmosRepoMock = new Mock<ICosmosDbRepository>();
            _mapperMock = new Mock<IMapper>();
            _activityRepoMock = new Mock<IActivityRepository>();
            _loggerMock = new Mock<ILogger<ActivityService>>();

            _mapperMock.Setup(x => x.Map(It.IsAny<res.Activity>(), It.IsAny<ActivityRecord>())).Verifiable();
            _mapperMock.Setup(x => x.Map(It.IsAny<res.Distance>(), It.IsAny<ActivityDistancesRecord>())).Verifiable();
            _mapperMock.Setup(x => x.Map(It.IsAny<res.HeartRateZone>(), It.IsAny<ActivityHeartRateZonesRecord>())).Verifiable();
            _mapperMock.Setup(x => x.Map(It.IsAny<res.Summary>(), It.IsAny<ActivitySummaryRecord>())).Verifiable();

            _sut = new ActivityService(
                _serviceBusMock.Object,
                _loggerMock.Object,
                _mapperMock.Object,
                _activityRepoMock.Object,
                _cosmosRepoMock.Object);
        }

        [Fact]
        public async Task MapAndSaveActivityEnvelopeSuccessfully()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityResponse = fixture.Create<res.ActivityResponse>();
            var date = "2022-12-31";

            _cosmosRepoMock
                .Setup(x => x.CreateActivityDocument(It.IsAny<ActivityEnvelope>()))
                .Returns(Task.CompletedTask);

            // ACT
            Func<Task> activityServiceAction = async () => await _sut.MapActivityEnvelopeAndSaveToDatabase(date, activityResponse);

            // ASSERT
            await activityServiceAction.Should().NotThrowAsync<Exception>();
            _cosmosRepoMock.Verify(x => x.CreateActivityDocument(It.IsAny<ActivityEnvelope>()), Times.Once);
        }

        [Fact]
        public async Task ThrowExceptionWhenMapAndSaveActivityEnvelopeFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityResponse = fixture.Create<res.ActivityResponse>();
            var date = "2022-12-31";

            _cosmosRepoMock
                .Setup(x => x.CreateActivityDocument(It.IsAny<ActivityEnvelope>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> activityServiceAction = async () => await _sut.MapActivityEnvelopeAndSaveToDatabase(date, activityResponse);

            // ASSERT
            await activityServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in MapActivityEnvelopeAndSaveToDatabase: Mock Failure"));
        }

        [Fact]
        public async Task MapAndSaveActivityHeartRateRecordSuccessfully()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            activityEnvelope.Date = "2023-01-01";

            _activityRepoMock
                .Setup(x => x.AddActivityHeartRateZoneRecord(It.IsAny<ActivityHeartRateZonesRecord>()))
                .Returns(Task.CompletedTask);

            // ACT
            Func<Task> activityServiceAction = async () => await _sut.MapAndSaveActivityHeartRateRecord(activityEnvelope);

            // ASSERT
            await activityServiceAction.Should().NotThrowAsync<Exception>();
        }

        [Fact]
        public async Task ThrowExceptionWhenHeartRateZonesAreNull()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            activityEnvelope.Date = "2023-01-01";
            activityEnvelope.Activity.summary.heartRateZones = null;

            // ACT
            Func<Task> activityServiceAction = async () => await _sut.MapAndSaveActivityHeartRateRecord(activityEnvelope);

            // ASSERT
            await activityServiceAction.Should().ThrowAsync<NullReferenceException>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in MapAndSaveActivityHeartRateRecord: No Heart Rate zone records to map!"));
        }

        [Fact]
        public async Task ThrowExceptionWhenAddActivityHeartRateZoneRecordFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            activityEnvelope.Date = "2023-01-01";

            _activityRepoMock
                .Setup(x => x.AddActivityHeartRateZoneRecord(It.IsAny<ActivityHeartRateZonesRecord>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> activityServiceAction = async () => await _sut.MapAndSaveActivityHeartRateRecord(activityEnvelope);

            // ASSERT
            await activityServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in MapAndSaveActivityHeartRateRecord: Mock Failure"));
        }

        [Fact]
        public async Task MapAndSaveActivityDistanceRecordSuccessfully()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            activityEnvelope.Date = "2023-01-01";

            _activityRepoMock
                .Setup(x => x.AddActivityDistancesRecord(It.IsAny<ActivityDistancesRecord>()))
                .Returns(Task.CompletedTask);

            // ACT
            Func<Task> activityServiceAction = async () => await _sut.MapAndSaveActivityDistanceRecord(activityEnvelope);

            // ASSERT
            await activityServiceAction.Should().NotThrowAsync<Exception>();
        }

        [Fact]
        public async Task ThrowExceptionWhenDistancesAreNull()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            activityEnvelope.Date = "2023-01-01";
            activityEnvelope.Activity.summary.distances = null;

            // ACT
            Func<Task> activityServiceAction = async () => await _sut.MapAndSaveActivityDistanceRecord(activityEnvelope);

            // ASSERT
            await activityServiceAction.Should().ThrowAsync<NullReferenceException>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in MapAndSaveActivityDistanceRecord: No Distances to map!"));
        }

        [Fact]
        public async Task ThrowExceptionWhenAddActivityDistancesRecordFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            activityEnvelope.Date = "2023-01-01";

            _activityRepoMock
                .Setup(x => x.AddActivityDistancesRecord(It.IsAny<ActivityDistancesRecord>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> activityServiceAction = async () => await _sut.MapAndSaveActivityDistanceRecord(activityEnvelope);

            // ASSERT
            await activityServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in MapAndSaveActivityDistanceRecord: Mock Failure"));
        }

        [Fact]
        public async Task MapAndSaveActivityRecordsSuccessfully()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            activityEnvelope.Date = "2023-01-01";

            _activityRepoMock
                .Setup(x => x.AddActivityRecord(It.IsAny<ActivityRecord>()))
                .Returns(Task.CompletedTask);

            // ACT
            Func<Task> activityServiceAction = async () => await _sut.MapAndSaveActivityRecords(activityEnvelope);

            // ASSERT
            await activityServiceAction.Should().NotThrowAsync<Exception>();
        }

        [Fact]
        public async Task NotThrowExceptionWhenActivitiesAreNull()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            activityEnvelope.Date = "2023-01-01";
            activityEnvelope.Activity.activities = null;

            // ACT
            Func<Task> activityServiceAction = async () => await _sut.MapAndSaveActivityRecords(activityEnvelope);

            // ASSERT
            await activityServiceAction.Should().NotThrowAsync<NullReferenceException>();
            _loggerMock.VerifyLog(logger => logger.LogInformation($"Must have been a rest day!"));
        }

        [Fact]
        public async Task ThrowExceptionWhenAddActivityRecordFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            activityEnvelope.Date = "2023-01-01";

            _activityRepoMock
                .Setup(x => x.AddActivityRecord(It.IsAny<ActivityRecord>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> activityServiceAction = async () => await _sut.MapAndSaveActivityRecords(activityEnvelope);

            // ASSERT
            await activityServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in MapAndSaveActivityRecords: Mock Failure"));
        }

        [Fact]
        public async Task MapAndSaveActivitySummaryRecordSuccessfully()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            activityEnvelope.Date = "2023-01-01";
            var heartRateZoneId = 1;
            var distanceId = 1;

            _activityRepoMock
                .Setup(x => x.GetActivityHeartRateZoneId())
                .ReturnsAsync(heartRateZoneId);

            _activityRepoMock
                .Setup(x => x.GetActivityDistanceId())
                .ReturnsAsync(distanceId);

            _activityRepoMock
                .Setup(x => x.AddActivitySummaryRecord(It.IsAny<ActivitySummaryRecord>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            // ACT
            Func<Task> activityServiceAction = async () => await _sut.MapAndSaveActivitySummaryRecord(activityEnvelope);

            // ASSERT
            await activityServiceAction.Should().NotThrowAsync<Exception>();
            _activityRepoMock.Verify(x => x.GetActivityHeartRateZoneId(), Times.Once);
            _activityRepoMock.Verify(x => x.GetActivityDistanceId(), Times.Once);
        }

        [Fact]
        public async Task ThrowExceptionWhenSummaryAreNull()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            activityEnvelope.Date = "2023-01-01";
            activityEnvelope.Activity.summary = null;

            // ACT
            Func<Task> activityServiceAction = async () => await _sut.MapAndSaveActivitySummaryRecord(activityEnvelope);

            // ASSERT
            await activityServiceAction.Should().ThrowAsync<NullReferenceException>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in MapAndSaveActivitySummaryRecord: No Activity Summaries to map!"));
        }

        [Fact]
        public async Task ThrowExceptionWhenAddActivitySummaryRecordFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            activityEnvelope.Date = "2023-01-01";
            var heartRateZoneId = 1;
            var distanceId = 1;

            _activityRepoMock
                .Setup(x => x.GetActivityHeartRateZoneId())
                .ReturnsAsync(heartRateZoneId);

            _activityRepoMock
                .Setup(x => x.GetActivityDistanceId())
                .ReturnsAsync(distanceId);

            _activityRepoMock
                .Setup(x => x.AddActivitySummaryRecord(It.IsAny<ActivitySummaryRecord>(), It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> activityServiceAction = async () => await _sut.MapAndSaveActivitySummaryRecord(activityEnvelope);

            // ASSERT
            await activityServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in MapAndSaveActivitySummaryRecord: Mock Failure"));
        }

        [Fact]
        public async Task ThrowExceptionWhenGetActivityDistanceIdFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            activityEnvelope.Date = "2023-01-01";
            var heartRateZoneId = 1;

            _activityRepoMock
                .Setup(x => x.GetActivityHeartRateZoneId())
                .ReturnsAsync(heartRateZoneId);

            _activityRepoMock
                .Setup(x => x.GetActivityDistanceId())
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> activityServiceAction = async () => await _sut.MapAndSaveActivitySummaryRecord(activityEnvelope);

            // ASSERT
            await activityServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in MapAndSaveActivitySummaryRecord: Mock Failure"));
        }

        [Fact]
        public async Task ThrowExceptionWhenGetActivityHeartRateZoneIdFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityEnvelope = fixture.Create<ActivityEnvelope>();
            activityEnvelope.Date = "2023-01-01";

            _activityRepoMock
                .Setup(x => x.GetActivityHeartRateZoneId())
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> activityServiceAction = async () => await _sut.MapAndSaveActivitySummaryRecord(activityEnvelope);

            // ASSERT
            await activityServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in MapAndSaveActivitySummaryRecord: Mock Failure"));
        }

        [Fact]
        public async Task SendActivityRecordToQueue()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityResponse = fixture.Create<res.ActivityResponse>();
            var queueName = "activityqueue";

            _serviceBusMock
                .Setup(x => x.CreateSender(It.IsAny<string>()))
                .Returns(_serviceBusSenderMock.Object);

            _serviceBusSenderMock
                .Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // ACT
            Func<Task> activityServiceAction = async () => await _sut.SendRecordToQueue(activityResponse, queueName);

            // ASSERT
            await activityServiceAction.Should().NotThrowAsync<Exception>();
            _serviceBusSenderMock.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ThrowExceptionWhenSendingRecordToQueueFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityResponse = fixture.Create<res.ActivityResponse>();
            var queueName = "activityqueue";

            _serviceBusMock
                .Setup(x => x.CreateSender(It.IsAny<string>()))
                .Returns(_serviceBusSenderMock.Object);

            _serviceBusSenderMock
                .Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> activityServiceAction = async () => await _sut.SendRecordToQueue(activityResponse, queueName);

            // ASSERT
            await activityServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in SendRecordToQueue: Mock Failure"));
        }
    }
}
