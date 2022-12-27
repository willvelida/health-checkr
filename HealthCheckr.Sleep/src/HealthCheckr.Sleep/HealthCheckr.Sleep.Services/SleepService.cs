using Azure.Messaging.ServiceBus;
using HealthCheckr.Sleep.Common;
using HealthCheckr.Sleep.Common.Envelopes;
using HealthCheckr.Sleep.Common.FitbitResponses;
using HealthCheckr.Sleep.Repository.Interfaces;
using HealthCheckr.Sleep.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HealthCheckr.Sleep.Services
{
    public class SleepService : ISleepService
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ICosmosDbRepository _cosmosDbRepository;
        private readonly Settings _settings;
        private readonly ILogger<SleepService> _logger;

        public SleepService(ServiceBusClient serviceBusClient, ICosmosDbRepository cosmosDbRepository, IOptions<Settings> options, ILogger<SleepService> logger)
        {
            _settings = options.Value;
            _serviceBusClient = serviceBusClient;
            _cosmosDbRepository = cosmosDbRepository;
            _logger = logger;
        }

        public async Task<List<SleepEnvelope>> GetAllSleepRecords()
        {
            try
            {
                return await _cosmosDbRepository.GetSleepEnvelopes();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetAllSleepRecords)}: {ex.Message}");
                throw;
            }
        }

        public async Task<SleepEnvelope> GetSleepRecordByDate(string sleepDate)
        {
            try
            {
                return await _cosmosDbRepository.GetSleepEnvelopeByDate(sleepDate);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetSleepRecordByDate)}: {ex.Message}");
                throw;
            }
        }

        public async Task MapAndSendSleepRecordToQueue(SleepResponseObject sleepResponse)
        {
            try
            {
                ServiceBusSender serviceBusSender = _serviceBusClient.CreateSender(_settings.SleepQueueName);
                var messageAsJson = JsonConvert.SerializeObject(sleepResponse);
                await serviceBusSender.SendMessageAsync(new ServiceBusMessage(messageAsJson));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapAndSendSleepRecordToQueue)}: {ex.Message}");
                throw;
            }
        }

        public async Task MapSleepEnvelopeAndSaveToDatabase(SleepResponseObject sleepResponse)
        {
            try
            {
                var sleepEnvelope = new SleepEnvelope
                {
                    Id = Guid.NewGuid().ToString(),
                    Sleep = sleepResponse,
                    DocumentType = "Sleep",
                    Date = sleepResponse.sleep[0].dateOfSleep
                };

                await _cosmosDbRepository.CreateSleepDocument(sleepEnvelope);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapSleepEnvelopeAndSaveToDatabase)}: {ex.Message}");
                throw;
            }
        }
    }
}
