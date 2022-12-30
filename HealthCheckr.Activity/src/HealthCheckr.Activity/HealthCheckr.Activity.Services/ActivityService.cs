using Azure.Messaging.ServiceBus;
using HealthCheckr.Activity.Common;
using HealthCheckr.Activity.Common.FitbitResponses;
using HealthCheckr.Activity.Repository.Interfaces;
using HealthCheckr.Activity.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using env = HealthCheckr.Activity.Common.Envelopes;

namespace HealthCheckr.Activity.Services
{
    public class ActivityService : IActivityService
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ICosmosDbRepository _cosmosDbRepository;
        private readonly Settings _settings;
        private readonly ILogger<ActivityService> _logger;

        public ActivityService(
            ServiceBusClient serviceBusClient,
            ILogger<ActivityService> logger,
            IOptions<Settings> options,
            ICosmosDbRepository cosmosDbRepository)
        {
            _serviceBusClient = serviceBusClient;
            _logger = logger;
            _settings = options.Value;
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

        public async Task MapAndSendActivityRecordToQueue(ActivityResponse activityResponse)
        {
            try
            {
                ServiceBusSender serviceBusSender = _serviceBusClient.CreateSender(_settings.ActivityQueueName);
                var messageAsJson = JsonConvert.SerializeObject(activityResponse);
                await serviceBusSender.SendMessageAsync(new ServiceBusMessage(messageAsJson));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapAndSendActivityRecordToQueue)}: {ex.Message}");
                throw;
            }
        }
    }
}
