using AutoFixture;
using Azure.Messaging.ServiceBus;
using FluentAssertions;
using HealthCheckr.Body.Common.Envelopes;
using HealthCheckr.Body.Common.FitbitResponses;
using HealthCheckr.Body.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace HealthCheckr.Body.Services.UnitTests
{
    public class BodyServiceShould
    {
        private Mock<ServiceBusClient> _serviceBusMock;
        private Mock<ServiceBusSender> _serviceBusSenderMock;
        private Mock<ICosmosDbRepository> _cosmosRepoMock;
        private Mock<ILogger<BodyService>> _loggerMock;

        private BodyService _sut;

        public BodyServiceShould()
        {
            _serviceBusMock = new Mock<ServiceBusClient>();
            _serviceBusSenderMock = new Mock<ServiceBusSender>();
            _cosmosRepoMock= new Mock<ICosmosDbRepository>();
            _loggerMock = new Mock<ILogger<BodyService>>();

            _sut = new BodyService(_serviceBusMock.Object, _cosmosRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task MapAndSaveWeightEnvelopeSuccessfully()
        {
            // ARRANGE
            var fixture = new Fixture();
            var weight = fixture.Create<Weight>();
            weight.date = "2022-12-31";

            _cosmosRepoMock
                .Setup(x => x.CreateWeightDocument(It.IsAny<WeightEnvelope>()))
                .Returns(Task.CompletedTask);

            // ACT
            Func<Task> bodyServiceAction = async () => await _sut.MapWeightEnvelopeAndSaveToDatabase(weight);

            // ASSERT
            await bodyServiceAction.Should().NotThrowAsync();
            _cosmosRepoMock.Verify(x => x.CreateWeightDocument(It.IsAny<WeightEnvelope>()), Times.Once);
        }

        [Fact]
        public async Task MapAndSaveV02EnvelopeSuccessfully()
        {
            // ARRANGE
            var fixture = new Fixture();
            var cardio = fixture.Create<CardioResponseObject>();
            cardio.cardioScore[0].dateTime = "2022-12-31";

            _cosmosRepoMock
                .Setup(x => x.CreateV02MaxDocument(It.IsAny<CardioEnvelope>()))
                .Returns(Task.CompletedTask);

            // ACT
            Func<Task> bodyServiceAction = async () => await _sut.MapCardioEnvelopeAndSaveToDatabase(cardio);

            // ASSERT
            await bodyServiceAction.Should().NotThrowAsync();
            _cosmosRepoMock.Verify(x => x.CreateV02MaxDocument(It.IsAny<CardioEnvelope>()), Times.Once);
        }

        [Fact]
        public async Task ThrowExceptionWhenMapWeightEnvelopeFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var weight = fixture.Create<Weight>();
            weight.date = "2022-12-31";

            _cosmosRepoMock
                .Setup(x => x.CreateWeightDocument(It.IsAny<WeightEnvelope>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> bodyServiceAction = async () => await _sut.MapWeightEnvelopeAndSaveToDatabase(weight);

            // ASSERT
            await bodyServiceAction.Should().NotThrowAsync();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in MapWeightEnvelopeAndSaveToDatabase: Mock Failure"));
        }

        [Fact]
        public async Task ThrowExceptionWhenMapCardioEnvelopeFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var cardio = fixture.Create<CardioResponseObject>();
            cardio.cardioScore[0].dateTime = "2022-12-31";

            _cosmosRepoMock
                .Setup(x => x.CreateV02MaxDocument(It.IsAny<CardioEnvelope>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> bodyServiceAction = async () => await _sut.MapCardioEnvelopeAndSaveToDatabase(cardio);

            // ASSERT
            await bodyServiceAction.Should().NotThrowAsync();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in MapCardioEnvelopeAndSaveToDatabase: Mock Failure"));
        }

        [Fact]
        public async Task SendRecordToQueueSuccessfully()
        {
            // ARRANGE
            var fixture = new Fixture();
            var weightResponse = fixture.Create<WeightEnvelope>();
            var queueName = "bodyqueue";

            _serviceBusMock
                .Setup(x => x.CreateSender(It.IsAny<string>()))
                .Returns(_serviceBusSenderMock.Object);

            _serviceBusSenderMock
                .Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // ACT
            Func<Task> activityServiceAction = async () => await _sut.SendRecordToQueue(weightResponse, queueName);

            // ASSERT
            await activityServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in SendRecordToQueue: Mock Failure"));
        }

        [Fact]
        public async Task ThrowExceptionWhenSendingRecordToQueueFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var weightResponse = fixture.Create<WeightEnvelope>();
            var queueName = "bodyqueue";

            _serviceBusMock
                .Setup(x => x.CreateSender(It.IsAny<string>()))
                .Returns(_serviceBusSenderMock.Object);

            _serviceBusSenderMock
                .Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> activityServiceAction = async () => await _sut.SendRecordToQueue(weightResponse, queueName);

            // ASSERT
            await activityServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in SendRecordToQueue: Mock Failure"));
        }
    }
}