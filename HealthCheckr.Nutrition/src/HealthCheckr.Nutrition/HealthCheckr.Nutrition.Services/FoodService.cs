using Azure.Messaging.ServiceBus;
using HealthCheckr.Nutrition.Common;
using HealthCheckr.Nutrition.Common.Envelopes;
using HealthCheckr.Nutrition.Common.FitbitResponses;
using HealthCheckr.Nutrition.Repository.Interfaces;
using HealthCheckr.Nutrition.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCheckr.Nutrition.Services
{
    public class FoodService : IFoodService
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ICosmosDbRepository _cosmosDbRepository;
        private readonly Settings _settings;
        private readonly ILogger<FoodService> _logger;

        public FoodService(ServiceBusClient serviceBusClient,
            ILogger<FoodService> logger,
            IOptions<Settings> options,
            ICosmosDbRepository cosmosDbRepository)
        {
            _serviceBusClient = serviceBusClient;
            _logger = logger;
            _settings = options.Value;
            _cosmosDbRepository = cosmosDbRepository;
        }

        public async Task MapFoodEnvelopeAndSaveToDatabase(FoodResponseObject foodResponseObject)
        {
            try
            {
                FoodEnvelope foodEnvelope = new FoodEnvelope
                {
                    Id = Guid.NewGuid().ToString(),
                    Food = foodResponseObject,
                    DocumentType = "Food",
                    Date = foodResponseObject.foods[0].logDate
                };

                await _cosmosDbRepository.CreateFoodDocument(foodEnvelope);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapFoodEnvelopeAndSaveToDatabase)}: {ex.Message}");
                throw;
            }
        }

        public async Task SendFoodRecordToQueue(FoodResponseObject foodResponseObject)
        {
            try
            {
                ServiceBusSender serviceBusSender = _serviceBusClient.CreateSender(_settings.NutritionQueueName);
                var messageAsJson = JsonConvert.SerializeObject(foodResponseObject);
                await serviceBusSender.SendMessageAsync(new ServiceBusMessage(messageAsJson));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(SendFoodRecordToQueue)}: {ex.Message}");
                throw;
            }
        }
    }
}
