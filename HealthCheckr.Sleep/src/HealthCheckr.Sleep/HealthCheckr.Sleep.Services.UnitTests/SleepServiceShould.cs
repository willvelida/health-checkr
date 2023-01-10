using AutoFixture;
using AutoMapper;
using Azure.Messaging.ServiceBus;
using FluentAssertions;
using HealthCheckr.Sleep.Common.Envelopes;
using HealthCheckr.Sleep.Common.FitbitResponses;
using HealthCheckr.Sleep.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCheckr.Sleep.Services.UnitTests
{
    public class SleepServiceShould
    {
        private Mock<ServiceBusClient> _serviceBusMock;
        private Mock<ServiceBusSender> _serviceBusSenderMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ISleepRepository> _sleepRepoMock;
        private Mock<ICosmosDbRepository> _cosmosRepoMock;
        private Mock<ILogger<SleepService>> _loggerMock;

        private SleepService _sut;

        public SleepServiceShould()
        {
            _serviceBusMock = new Mock<ServiceBusClient>();
            _serviceBusSenderMock = new Mock<ServiceBusSender>();
            _mapperMock = new Mock<IMapper>();
            _cosmosRepoMock = new Mock<ICosmosDbRepository>();
            _sleepRepoMock = new Mock<ISleepRepository>();
            _loggerMock = new Mock<ILogger<SleepService>>();

            _mapperMock.Setup(x => x.Map(It.IsAny<Sp02ResponseObject>(), It.IsAny<Sp02Record>())).Verifiable();
            _mapperMock.Setup(x => x.Map(It.IsAny<BreathingRateResponseObject>(), It.IsAny<BreathingRateRecord>())).Verifiable();

            _sut = new SleepService(
                _serviceBusMock.Object,
                _mapperMock.Object,
                _sleepRepoMock.Object,
                _cosmosRepoMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task MapAndSaveBreathingRecordSuccessfully()
        {
            // ARRANGE
            var fixture = new Fixture();
            var breathingRateResponseObject = fixture.Create<BreathingRateResponseObject>();

            _sleepRepoMock
                .Setup(x => x.AddBreathingRateRecord(It.IsAny<BreathingRateRecord>()))
                .Returns(Task.CompletedTask);

            // ACT
            Func<Task> sleepServiceAction = async () => await _sut.MapBreathingRecordAndSaveToDatabase(breathingRateResponseObject);

            // ASSERT
            await sleepServiceAction.Should().NotThrowAsync();
            _sleepRepoMock.Verify(x => x.AddBreathingRateRecord(It.IsAny<BreathingRateRecord>()), Times.Once);
        }

        [Fact]
        public async Task MapAndSaveSp02RecordSuccessfully()
        {
            // ARRANGE
            var fixture = new Fixture();
            var sp02ResponseObject = fixture.Create<Sp02ResponseObject>();

            _sleepRepoMock
                .Setup(x => x.AddSp02Record(It.IsAny<Sp02Record>()))
                .Returns(Task.CompletedTask);

            // ACT
            Func<Task> sleepServiceAction = async () => await _sut.MapSp02RecordAndSaveToDatabase(sp02ResponseObject);

            // ASSERT
            await sleepServiceAction.Should().NotThrowAsync();
            _sleepRepoMock.Verify(x => x.AddSp02Record(It.IsAny<Sp02Record>()), Times.Once);
        }

        [Fact]
        public async Task ThrowExceptionWhenMapBreathingRecordFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var breathingRateResponseObject = fixture.Create<BreathingRateResponseObject>();

            _sleepRepoMock
                .Setup(x => x.AddBreathingRateRecord(It.IsAny<BreathingRateRecord>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> sleepServiceAction = async () => await _sut.MapBreathingRecordAndSaveToDatabase(breathingRateResponseObject);

            // ASSERT
            await sleepServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in MapBreathingRecordAndSaveToDatabase: Mock Failure"));
        }

        [Fact]
        public async Task MapSleepEnvelopeAndSaveSuccessfully()
        {
            // ARRANGE
            var fixture = new Fixture();
            var sleepResponseObject = fixture.Create<SleepResponseObject>();
            sleepResponseObject.sleep[0].dateOfSleep = "2023-01-01";

            _cosmosRepoMock
                .Setup(x => x.CreateSleepDocument(It.IsAny<SleepEnvelope>()))
                .Returns(Task.CompletedTask);

            // ACT
            Func<Task> sleepServiceAction = async () => await _sut.MapSleepEnvelopeAndSaveToDatabase(sleepResponseObject);

            // ASSERT
            await sleepServiceAction.Should().NotThrowAsync();
            _cosmosRepoMock.Verify(x => x.CreateSleepDocument(It.IsAny<SleepEnvelope>()), Times.Once);
        }

        [Fact]
        public async Task ThrowExceptionWhenSaveSleepEnvelopeFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var sleepResponseObject = fixture.Create<SleepResponseObject>();
            sleepResponseObject.sleep[0].dateOfSleep = "2023-01-01";

            _cosmosRepoMock
                .Setup(x => x.CreateSleepDocument(It.IsAny<SleepEnvelope>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> sleepServiceAction = async () => await _sut.MapSleepEnvelopeAndSaveToDatabase(sleepResponseObject);

            // ASSERT
            await sleepServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in MapSleepEnvelopeAndSaveToDatabase: Mock Failure"));
        }

        [Fact]
        public async Task ThrowExceptionWhenMapSp02RecordFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var sp02ResponseObject = fixture.Create<Sp02ResponseObject>();

            _sleepRepoMock
                .Setup(x => x.AddSp02Record(It.IsAny<Sp02Record>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> sleepServiceAction = async () => await _sut.MapSp02RecordAndSaveToDatabase(sp02ResponseObject);

            // ASSERT
            await sleepServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in MapSp02RecordAndSaveToDatabase: Mock Failure"));
        }

        [Fact]
        public async Task SendRecordToQueueSuccessfully()
        {
            // ARRANGE
            var fixture = new Fixture();
            var sp02Response = fixture.Create<Sp02ResponseObject>();
            var queueName = "sp02queue";

            _serviceBusMock
                .Setup(x => x.CreateSender(It.IsAny<string>()))
                .Returns(_serviceBusSenderMock.Object);

            _serviceBusSenderMock
                .Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // ACT
            Func<Task> bodyServiceAction = async () => await _sut.SendRecordToQueue(sp02Response, queueName);

            // ASSERT
            await bodyServiceAction.Should().NotThrowAsync<Exception>();
            _serviceBusSenderMock.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ThrowExceptionWhenSendingRecordToQueueFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var sp02Response = fixture.Create<Sp02ResponseObject>();
            var queueName = "sp02queue";

            _serviceBusMock
                .Setup(x => x.CreateSender(It.IsAny<string>()))
                .Returns(_serviceBusSenderMock.Object);

            _serviceBusSenderMock
                .Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> bodyServiceAction = async () => await _sut.SendRecordToQueue(sp02Response, queueName);

            // ASSERT
            await bodyServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in SendRecordToQueue: Mock Failure"));
        }
    }
}
