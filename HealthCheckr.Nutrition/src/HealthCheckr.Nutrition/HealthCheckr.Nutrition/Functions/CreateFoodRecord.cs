using HealthCheckr.Nutrition.Common.FitbitResponses;
using HealthCheckr.Nutrition.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HealthCheckr.Nutrition.Functions
{
    public class CreateFoodRecord
    {
        private readonly IFoodService _foodService;
        private readonly ILogger<CreateFoodRecord> _logger;

        public CreateFoodRecord(IFoodService foodService, ILogger<CreateFoodRecord> logger)
        {
            _foodService = foodService;
            _logger = logger;
        }

        [Function(nameof(CreateFoodRecord))]
        public async Task Run([ServiceBusTrigger("nutritionqueue", Connection = "ServiceBusConnection")] string nutritionQueueItem)
        {
            try
            {
                var food = JsonConvert.DeserializeObject<FoodResponseObject>(nutritionQueueItem);

                _logger.LogInformation($"Persisting food message dated {food.foods[0].logDate} to database");
                await _foodService.MapFoodEnvelopeAndSaveToDatabase(food);
                _logger.LogInformation("Food message successfully persisted");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateFoodRecord)}: {ex.Message}");
                throw;
            }
        }
    }
}
