using Azure.Messaging.ServiceBus;
using HealthCheckr.Body.Common;
using HealthCheckr.Body.Common.Envelopes;
using HealthCheckr.Body.Common.FitbitResponses;
using HealthCheckr.Body.Repository.Interfaces;
using HealthCheckr.Body.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCheckr.Body.Services
{
    public class BodyService : IBodyService
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ICosmosDbRepository _cosmosDbRepository;
        private readonly Settings _settings;
        private readonly ILogger<BodyService> _logger;

        public BodyService(ServiceBusClient serviceBusClient, ICosmosDbRepository cosmosDbRepository, IOptions<Settings> options, ILogger<BodyService> logger)
        {
            _settings = options.Value;
            _serviceBusClient = serviceBusClient;
            _cosmosDbRepository = cosmosDbRepository;
            _logger = logger;
        }

        public async Task MapAndSendWeightRecordToQueue(Weight weight)
        {
            try
            {
                ServiceBusSender serviceBusSender = _serviceBusClient.CreateSender(_settings.BodyQueueName);
                var messageAsJson = JsonConvert.SerializeObject(weight);
                await serviceBusSender.SendMessageAsync(new ServiceBusMessage(messageAsJson));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapAndSendWeightRecordToQueue)}: {ex.Message}");
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
    }
}
