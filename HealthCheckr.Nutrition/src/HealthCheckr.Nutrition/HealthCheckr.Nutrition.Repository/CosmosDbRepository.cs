using HealthCheckr.Nutrition.Common;
using HealthCheckr.Nutrition.Common.Envelopes;
using HealthCheckr.Nutrition.Repository.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HealthCheckr.Nutrition.Repository
{
    public class CosmosDbRepository : ICosmosDbRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;
        private readonly Settings _settings;
        private readonly ILogger<CosmosDbRepository> _logger;

        public CosmosDbRepository(CosmosClient cosmosClient, IOptions<Settings> options, ILogger<CosmosDbRepository> logger)
        {
            _settings = options.Value;
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer(_settings.DatabaseName, _settings.NutritionContainerName);
            _logger = logger;
        }

        public async Task CreateFoodDocument(FoodEnvelope foodEnvelope)
        {
            try
            {
                ItemRequestOptions itemRequestOptions = new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = false
                };

                await _container.CreateItemAsync(foodEnvelope, new PartitionKey(foodEnvelope.Date), itemRequestOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateFoodDocument)}: {ex.Message}");
                throw;
            }
        }
    }
}
