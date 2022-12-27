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
        private readonly Container _container;
        private readonly Settings _settings;
        private readonly ILogger<CosmosDbRepository> _logger;

        public CosmosDbRepository(CosmosClient cosmosClient, Container container, IOptions<Settings> options, ILogger<CosmosDbRepository> logger)
        {
            _settings = options.Value;
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer(_settings.DatabaseName, _settings.ContainerName);
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

                await _container.CreateItemAsync(sleepEnvelope, new PartitionKey(sleepEnvelope.DocumentType), itemRequestOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateSleepDocument)}: {ex.Message}");
                throw;
            }
        }

        public async Task<SleepEnvelope> GetSleepEnvelopeByDate(string sleepDate)
        {
            try
            {
                QueryDefinition query = new QueryDefinition("SELECT * FROM Records c WHERE c.DocumentType = 'Sleep' AND c.Sleep.dateOfSleep = @sleepDate")
                    .WithParameter("@activityDate", sleepDate);
                QueryRequestOptions queryRequestOptions = new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey("DocumentType")
                };

                List<SleepEnvelope> sleepEnvelopes = new List<SleepEnvelope>();

                FeedIterator<SleepEnvelope> feedIterator = _container.GetItemQueryIterator<SleepEnvelope>(query, null, queryRequestOptions);

                while (feedIterator.HasMoreResults)
                {
                    FeedResponse<SleepEnvelope> queryResponse = await feedIterator.ReadNextAsync();
                    sleepEnvelopes.AddRange(queryResponse.Resource);
                }

                return sleepEnvelopes.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetSleepEnvelopeByDate)}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<SleepEnvelope>> GetSleepEnvelopes()
        {
            try
            {
                List<SleepEnvelope> sleepEnvelopes = new List<SleepEnvelope>();
                QueryDefinition query = new QueryDefinition("SELECT * FROM Records c WHERE c.DocumentType = 'Sleep'");
                QueryRequestOptions queryRequestOptions = new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey("DocumentType")
                };

                FeedIterator<SleepEnvelope> feedIterator = _container.GetItemQueryIterator<SleepEnvelope>(query, null, queryRequestOptions);

                while (feedIterator.HasMoreResults)
                {
                    FeedResponse<SleepEnvelope> queryResponse = await feedIterator.ReadNextAsync();
                    sleepEnvelopes.AddRange(queryResponse.Resource);
                }

                return sleepEnvelopes;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetSleepEnvelopes)}: {ex.Message}");
                throw;
            }
        }
    }
}
