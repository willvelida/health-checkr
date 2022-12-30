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

        public async Task MapBreathingEnvelopeAndSaveToDatabase(BreathingRateResponseObject breathingRateResponseObject)
        {
            try
            {
                var breathingEnvelope = new BreatingRateEnvelope
                {
                    Id = Guid.NewGuid().ToString(),
                    BreathingRate = breathingRateResponseObject,
                    Date = breathingRateResponseObject.br[0].dateTime,
                    DocumentType = "BreathingRate"
                };

                await _cosmosDbRepository.CreateBreathingRateDocument(breathingEnvelope);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapBreathingEnvelopeAndSaveToDatabase)}: {ex.Message}");
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

        public async Task MapSp02EnvelopeAndSaveToDatabase(Sp02ResponseObject sp02Response)
        {
            try
            {
                var sp02Envelope = new Sp02Envelope
                {
                    Id = Guid.NewGuid().ToString(),
                    Sp02 = sp02Response,
                    DocumentType = "SP02",
                    Date = sp02Response.dateTime
                };

                await _cosmosDbRepository.CreateSp02Document(sp02Envelope);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapSp02EnvelopeAndSaveToDatabase)}: {ex.Message}");
                throw;
            }
        }

        public async Task SendBreathingResponseToQueue(BreathingRateResponseObject breathingRateResponseObject)
        {
            try
            {
                ServiceBusSender serviceBusSender = _serviceBusClient.CreateSender(_settings.Sp02QueueName);
                var messageAsJson = JsonConvert.SerializeObject(breathingRateResponseObject);
                await serviceBusSender.SendMessageAsync(new ServiceBusMessage(messageAsJson));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(SendBreathingResponseToQueue)}: {ex.Message}");
                throw;
            }
        }

        public async Task SendSp02RecordToQueue(Sp02ResponseObject sp02Response)
        {
            try
            {
                ServiceBusSender serviceBusSender = _serviceBusClient.CreateSender(_settings.Sp02QueueName);
                var messageAsJson = JsonConvert.SerializeObject(sp02Response);
                await serviceBusSender.SendMessageAsync(new ServiceBusMessage(messageAsJson));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(SendSp02RecordToQueue)}: {ex.Message}");
                throw;
            }
        }
    }
}
