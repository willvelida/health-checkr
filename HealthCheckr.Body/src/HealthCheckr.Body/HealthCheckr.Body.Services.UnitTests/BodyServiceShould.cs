using AutoFixture;
using AutoMapper;
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
        private Mock<IMapper> _mapperMock;
        private Mock<IBodyRepository> _bodyRepoMock;
        private Mock<ILogger<BodyService>> _loggerMock;

        private BodyService _sut;

        public BodyServiceShould()
        {
            _serviceBusMock = new Mock<ServiceBusClient>();
            _serviceBusSenderMock = new Mock<ServiceBusSender>();
            _mapperMock = new Mock<IMapper>();
            _bodyRepoMock = new Mock<IBodyRepository>();
            _loggerMock = new Mock<ILogger<BodyService>>();

            _mapperMock.Setup(x => x.Map(It.IsAny<CardioResponseObject>(), It.IsAny<V02Record>())).Verifiable();
            _mapperMock.Setup(x => x.Map(It.IsAny<Weight>(), It.IsAny<WeightRecord>())).Verifiable();

            _sut = new BodyService(_serviceBusMock.Object, _mapperMock.Object, _bodyRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task MapAndSaveWeightEnvelopeSuccessfully()
        {
            // ARRANGE
            var fixture = new Fixture();
            var weight = fixture.Create<Weight>();
            weight.date = "2022-12-31";

            _bodyRepoMock
                .Setup(x => x.AddWeightRecord(It.IsAny<WeightRecord>()))
                .Returns(Task.CompletedTask);

            // ACT
            Func<Task> bodyServiceAction = async () => await _sut.MapWeightEnvelopeAndSaveToDatabase(weight);

            // ASSERT
            await bodyServiceAction.Should().NotThrowAsync();
            _bodyRepoMock.Verify(x => x.AddWeightRecord(It.IsAny<WeightRecord>()), Times.Once);
        }

        [Fact]
        public async Task MapAndSaveV02EnvelopeSuccessfully()
        {
            // ARRANGE
            var fixture = new Fixture();
            var cardio = fixture.Create<CardioResponseObject>();
            cardio.cardioScore[0].dateTime = "2022-12-31";

            _bodyRepoMock
                .Setup(x => x.AddV02Record(It.IsAny<V02Record>()))
                .Returns(Task.CompletedTask);

            // ACT
            Func<Task> bodyServiceAction = async () => await _sut.MapCardioEnvelopeAndSaveToDatabase(cardio);

            // ASSERT
            await bodyServiceAction.Should().NotThrowAsync();
            _bodyRepoMock.Verify(x => x.AddV02Record(It.IsAny<V02Record>()), Times.Once);
        }

        [Fact]
        public async Task ThrowExceptionWhenMapWeightEnvelopeFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var weight = fixture.Create<Weight>();
            weight.date = "2022-12-31";

            _bodyRepoMock
                .Setup(x => x.AddWeightRecord(It.IsAny<WeightRecord>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> bodyServiceAction = async () => await _sut.MapWeightEnvelopeAndSaveToDatabase(weight);

            // ASSERT
            await bodyServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in MapWeightEnvelopeAndSaveToDatabase: Mock Failure"));
        }

        [Fact]
        public async Task ThrowExceptionWhenMapCardioEnvelopeFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var cardio = fixture.Create<CardioResponseObject>();
            cardio.cardioScore[0].dateTime = "2022-12-31";

            _bodyRepoMock
                .Setup(x => x.AddV02Record(It.IsAny<V02Record>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> bodyServiceAction = async () => await _sut.MapCardioEnvelopeAndSaveToDatabase(cardio);

            // ASSERT
            await bodyServiceAction.Should().ThrowAsync<Exception>();
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
            Func<Task> bodyServiceAction = async () => await _sut.SendRecordToQueue(weightResponse, queueName);

            // ASSERT
            await bodyServiceAction.Should().NotThrowAsync<Exception>();
            _serviceBusSenderMock.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Once);
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
            Func<Task> bodyServiceAction = async () => await _sut.SendRecordToQueue(weightResponse, queueName);

            // ASSERT
            await bodyServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in SendRecordToQueue: Mock Failure"));
        }
    }
}