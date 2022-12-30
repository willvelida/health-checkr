using Azure.Messaging.ServiceBus;
using HealthCheckr.Activity.Common.FitbitResponses;
using HealthCheckr.Activity.Repository.Interfaces;
using HealthCheckr.Activity.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using env = HealthCheckr.Activity.Common.Envelopes;

namespace HealthCheckr.Activity.Services
{
    public class ActivityService : IActivityService
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ICosmosDbRepository _cosmosDbRepository;
        private readonly ILogger<ActivityService> _logger;

        public ActivityService(
            ServiceBusClient serviceBusClient,
            ILogger<ActivityService> logger,
            ICosmosDbRepository cosmosDbRepository)
        {
            _serviceBusClient = serviceBusClient;
            _logger = logger;
            _cosmosDbRepository = cosmosDbRepository;
        }

        public async Task MapActivityEnvelopeAndSaveToDatabase(string date, ActivityResponse activityResponse)
        {
            try
            {
                env.ActivityEnvelope activityEnvelope = new env.ActivityEnvelope
                {
                    Id = Guid.NewGuid().ToString(),
                    Activity = activityResponse,
                    DocumentType = "Activity",
                    Date = date
                };

                await _cosmosDbRepository.CreateActivityDocument(activityEnvelope);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapActivityEnvelopeAndSaveToDatabase)}: {ex.Message}");
                throw;
            }
        }

        public async Task MapHeartRateEnvelopeAndSaveToDatabase(HeartRateTimeSeriesResponse heartRateTimeSeriesResponse)
        {
            try
            {
                env.HeartRateEnvelope heartRateEnvelope = new env.HeartRateEnvelope
                {
                    Id = Guid.NewGuid().ToString(),
                    HeartRate = heartRateTimeSeriesResponse,
                    Date = heartRateTimeSeriesResponse.activitiesheart[0].dateTime,
                    DocumentType = "HeartRate"
                };

                await _cosmosDbRepository.CreateHeartRateDocument(heartRateEnvelope);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapHeartRateEnvelopeAndSaveToDatabase)}: {ex.Message}");
                throw;
            }
        }

        public async Task SendRecordToQueue<T>(T record, string queueName)
        {
            try
            {
                ServiceBusSender serviceBusSender = _serviceBusClient.CreateSender(queueName);
                var messageAsJson = JsonConvert.SerializeObject(record);
                await serviceBusSender.SendMessageAsync(new ServiceBusMessage(messageAsJson));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(SendRecordToQueue)}: {ex.Message}");
                throw;
            }
        }
    }
}
