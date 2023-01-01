using Azure.Messaging.ServiceBus;
using HealthCheckr.Body.Common;
using HealthCheckr.Body.Common.Envelopes;
using HealthCheckr.Body.Common.FitbitResponses;
using HealthCheckr.Body.Repository.Interfaces;
using HealthCheckr.Body.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HealthCheckr.Body.Services
{
    public class BodyService : IBodyService
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ICosmosDbRepository _cosmosDbRepository;
        private readonly ILogger<BodyService> _logger;

        public BodyService(ServiceBusClient serviceBusClient, ICosmosDbRepository cosmosDbRepository, ILogger<BodyService> logger)
        {
            _serviceBusClient = serviceBusClient;
            _cosmosDbRepository = cosmosDbRepository;
            _logger = logger;
        }

        public async Task MapCardioEnvelopeAndSaveToDatabase(CardioResponseObject cardioResponseObject)
        {
            try
            {
                var cardioEnvelope = new CardioEnvelope
                {
                    Id = Guid.NewGuid().ToString(),
                    Cardio = cardioResponseObject,
                    Date = cardioResponseObject.cardioScore[0].dateTime,
                    DocumentType = "V02"
                };

                await _cosmosDbRepository.CreateV02MaxDocument(cardioEnvelope);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapCardioEnvelopeAndSaveToDatabase)}: {ex.Message}");
                throw;
            }
        }

        public async Task MapWeightEnvelopeAndSaveToDatabase(Weight weight)
        {
            try
            {
                var weightEnvelope = new WeightEnvelope
                {
                    Id = Guid.NewGuid().ToString(),
                    Weight = weight,
                    DocumentType = "Weight",
                    Date = weight.date
                };

                await _cosmosDbRepository.CreateWeightDocument(weightEnvelope);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapWeightEnvelopeAndSaveToDatabase)}: {ex.Message}");
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
