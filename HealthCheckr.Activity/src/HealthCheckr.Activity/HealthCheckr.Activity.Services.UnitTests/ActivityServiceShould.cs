using AutoFixture;
using Azure.Messaging.ServiceBus;
using FluentAssertions;
using HealthCheckr.Activity.Common.Envelopes;
using HealthCheckr.Activity.Common.FitbitResponses;
using HealthCheckr.Activity.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCheckr.Activity.Services.UnitTests
{
    public class ActivityServiceShould
    {
        private Mock<ServiceBusClient> _serviceBusMock;
        private Mock<ServiceBusSender> _serviceBusSenderMock;
        private Mock<ICosmosDbRepository> _cosmosRepoMock;
        private Mock<ILogger<ActivityService>> _loggerMock;

        private ActivityService _sut;

        public ActivityServiceShould()
        {
            _serviceBusMock = new Mock<ServiceBusClient>();
            _serviceBusSenderMock = new Mock<ServiceBusSender>();
            _cosmosRepoMock = new Mock<ICosmosDbRepository>();
            _loggerMock = new Mock<ILogger<ActivityService>>();

            _sut = new ActivityService(
                _serviceBusMock.Object,
                _loggerMock.Object,
                _cosmosRepoMock.Object);
        }

        [Fact]
        public async Task MapAndSaveActivityEnvelopeSuccessfully()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityResponse = fixture.Create<ActivityResponse>();
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
        public async Task MapAndSaveHeartRateEnvelopeSuccesfully()
        {
            // ARRANGE
            var fixture = new Fixture();
            var heartRateResponse = fixture.Create<HeartRateTimeSeriesResponse>();
            heartRateResponse.activitiesheart[0].dateTime = "2022-12-31";

            _cosmosRepoMock
                .Setup(x => x.CreateHeartRateDocument(It.IsAny<HeartRateEnvelope>()))
                .Returns(Task.CompletedTask);

            // ACT
            Func<Task> activityServiceAction = async () => await _sut.MapHeartRateEnvelopeAndSaveToDatabase(heartRateResponse);

            // ASSERT
            await activityServiceAction.Should().NotThrowAsync<Exception>();
            _cosmosRepoMock.Verify(x => x.CreateHeartRateDocument(It.IsAny<HeartRateEnvelope>()), Times.Once);
        }

        [Fact]
        public async Task ThrowExceptionWhenMapAndSaveActivityEnvelopeFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityResponse = fixture.Create<ActivityResponse>();
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
        public async Task ThrowExceptionWhenMapAndSaveHeartRateEnvelopeFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var heartRateResponse = fixture.Create<HeartRateTimeSeriesResponse>();
            heartRateResponse.activitiesheart[0].dateTime = "2022-12-31";

            _cosmosRepoMock
                .Setup(x => x.CreateHeartRateDocument(It.IsAny<HeartRateEnvelope>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> activityServiceAction = async () => await _sut.MapHeartRateEnvelopeAndSaveToDatabase(heartRateResponse);

            // ASSERT
            await activityServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in MapHeartRateEnvelopeAndSaveToDatabase: Mock Failure"));
        }

        [Fact]
        public async Task SendActivityRecordToQueue()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activityResponse = fixture.Create<ActivityResponse>();
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
            var activityResponse = fixture.Create<ActivityResponse>();
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
