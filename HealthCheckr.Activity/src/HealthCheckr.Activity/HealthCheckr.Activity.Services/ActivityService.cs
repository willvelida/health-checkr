using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ICosmosDbRepository _cosmosDbRepository;
        private readonly Settings _settings;
        private readonly ILogger<ActivityService> _logger;

        public ActivityService(
            IMapper mapper,
            ServiceBusClient serviceBusClient,
            ILogger<ActivityService> logger,
            IOptions<Settings> options,
            ICosmosDbRepository cosmosDbRepository)
        {
            _mapper = mapper;
            _serviceBusClient = serviceBusClient;
            _logger = logger;
            _settings = options.Value;
            _cosmosDbRepository = cosmosDbRepository;
        }

        public async Task<env.ActivityEnvelope> GetActivityRecordByDate(string activityDate)
        {
            try
            {
                var activityRecord = await _cosmosDbRepository.GetActivityEnvelopeByDate(activityDate);

                return activityRecord;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetActivityRecordByDate)}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<env.ActivityEnvelope>> GetAllActivityRecords()
        {
            try
            {
                var activityRecords = await _cosmosDbRepository.GetActivities();

                return activityRecords;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetAllActivityRecords)}: {ex.Message}");
                throw;
            }
        }

        public async Task MapActivityEnvelopeAndSaveToDatabase(env.Activity activity)
        {
            try
            {
                env.ActivityEnvelope activityEnvelope = new env.ActivityEnvelope
                {
                    Id = Guid.NewGuid().ToString(),
                    Activity = activity,
                    DocumentType = "Activity",
                    Date = activity.ActivityDate
                };

                await _cosmosDbRepository.CreateActivityDocument(activityEnvelope);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapActivityEnvelopeAndSaveToDatabase)}: {ex.Message}");
                throw;
            }
        }

        public async Task MapAndSendActivityRecordToQueue(string date, ActivityResponse activityResponse)
        {
            try
            {
                var activity = new env.Activity();
                activity.ActivityDate = date;
                _mapper.Map(activityResponse, activity);

                ServiceBusSender serviceBusSender = _serviceBusClient.CreateSender(_settings.ActivityQueueName);
                var messageAsJson = JsonConvert.SerializeObject(activity);
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
