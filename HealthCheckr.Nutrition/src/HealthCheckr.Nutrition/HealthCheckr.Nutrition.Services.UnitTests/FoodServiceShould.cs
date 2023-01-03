using AutoFixture;
using Azure.Messaging.ServiceBus;
using FluentAssertions;
using HealthCheckr.Nutrition.Common;
using HealthCheckr.Nutrition.Common.Envelopes;
using HealthCheckr.Nutrition.Common.FitbitResponses;
using HealthCheckr.Nutrition.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace HealthCheckr.Nutrition.Services.UnitTests
{
    public class FoodServiceShould
    {
        private Mock<ServiceBusClient> _serviceBusMock;
        private Mock<ServiceBusSender> _serviceBusSenderMock;
        private Mock<ICosmosDbRepository> _cosmosRepoMock;
        private Mock<IOptions<Settings>> _optionsMock;
        private Mock<ILogger<FoodService>> _loggerMock;

        private FoodService _sut;

        public FoodServiceShould()
        {
            _serviceBusMock = new Mock<ServiceBusClient>();
            _serviceBusSenderMock = new Mock<ServiceBusSender>();
            _cosmosRepoMock = new Mock<ICosmosDbRepository>();
            _optionsMock = new Mock<IOptions<Settings>>();
            _loggerMock = new Mock<ILogger<FoodService>>();

            Settings settings = new Settings
            {
                DatabaseName = "testdb",
                NutritionContainerName = "nutritioncontainer",
                NutritionQueueName = "nutritionqueue"
            };

            _optionsMock
                .Setup(x => x.Value)
                .Returns(settings);

            _serviceBusMock
                .Setup(x => x.CreateSender(It.IsAny<string>()))
                .Returns(_serviceBusSenderMock.Object);

            _sut = new FoodService(
                _serviceBusMock.Object,
                _loggerMock.Object,
                _optionsMock.Object,
                _cosmosRepoMock.Object);
        }

        [Fact]
        public async Task MapAndSaveFoodEnvelopeSuccessfully()
        {
            // ARRANGE
            var fixture = new Fixture();
            var foodResponse = fixture.Create<FoodResponseObject>();
            foodResponse.foods[0].logDate = "2023-01-01";

            _cosmosRepoMock
                .Setup(x => x.CreateFoodDocument(It.IsAny<FoodEnvelope>()))
                .Returns(Task.CompletedTask);

            // ACT
            Func<Task> foodServiceAction = async () => await _sut.MapFoodEnvelopeAndSaveToDatabase(foodResponse);

            // ASSERT
            await foodServiceAction.Should().NotThrowAsync<Exception>();
            _cosmosRepoMock.Verify(x => x.CreateFoodDocument(It.IsAny<FoodEnvelope>()), Times.Once);
        }

        [Fact]
        public async Task ThrowExceptionWhenSaveFoodEnvelopeFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var foodResponse = fixture.Create<FoodResponseObject>();
            foodResponse.foods[0].logDate = "2023-01-01";

            _cosmosRepoMock
                .Setup(x => x.CreateFoodDocument(It.IsAny<FoodEnvelope>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> foodServiceAction = async () => await _sut.MapFoodEnvelopeAndSaveToDatabase(foodResponse);

            // ASSERT
            await foodServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in MapFoodEnvelopeAndSaveToDatabase: Mock Failure"));
        }

        [Fact]
        public async Task SendFoodResponseObjectToQueue()
        {
            // ARRANGE
            var fixture = new Fixture();
            var foodResponse = fixture.Create<FoodResponseObject>();

            _serviceBusSenderMock
                .Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // ACT
            Func<Task> foodServiceAction = async () => await _sut.SendFoodRecordToQueue(foodResponse);

            // ASSERT
            await foodServiceAction.Should().NotThrowAsync<Exception>();
            _serviceBusSenderMock
                .Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ThrowExceptionWhenSendingRecordToQueueFails()
        {
            // ARRANGE
            var fixture = new Fixture();
            var foodResponse = fixture.Create<FoodResponseObject>();

            _serviceBusSenderMock
                .Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Mock Failure"));

            // ACT
            Func<Task> foodServiceAction = async () => await _sut.SendFoodRecordToQueue(foodResponse);
            
            // ASSERT
            await foodServiceAction.Should().ThrowAsync<Exception>();
            _loggerMock.VerifyLog(logger => logger.LogError($"Exception thrown in SendFoodRecordToQueue: Mock Failure"));
        }
    }
}