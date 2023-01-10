using HealthCheckr.Sleep.Common;
using HealthCheckr.Sleep.Common.Envelopes;
using HealthCheckr.Sleep.Repository.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HealthCheckr.Sleep.Repository
{
    public class CosmosDbRepository : ICosmosDbRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _recordContainer;
        private readonly Container _sleepContainer;
        private readonly Settings _settings;
        private readonly ILogger<CosmosDbRepository> _logger;

        public CosmosDbRepository(CosmosClient cosmosClient, IOptions<Settings> options, ILogger<CosmosDbRepository> logger)
        {
            _settings = options.Value;
            _cosmosClient = cosmosClient;
            _recordContainer = _cosmosClient.GetContainer(_settings.DatabaseName, _settings.ContainerName);
            _sleepContainer = _cosmosClient.GetContainer(_settings.DatabaseName, _settings.SleepContainerName);
            _logger = logger;
        }

        public async Task CreateSleepDocument(SleepEnvelope sleepEnvelope)
        {
            try
            {
                ItemRequestOptions itemRequestOptions = new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = false
                };

                await _sleepContainer.CreateItemAsync(sleepEnvelope, new PartitionKey(sleepEnvelope.Date), itemRequestOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateSleepDocument)}: {ex.Message}");
                throw;
            }
        }
    }
}
