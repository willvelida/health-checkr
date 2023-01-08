using AutoMapper;
using Azure.Messaging.ServiceBus;
using HealthCheckr.Body.Common.Envelopes;
using HealthCheckr.Body.Common.FitbitResponses;
using HealthCheckr.Body.Repository.Interfaces;
using HealthCheckr.Body.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HealthCheckr.Body.Services
{
    public class BodyService : IBodyService
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly IMapper _mapper;
        private readonly IBodyRepository _bodyRepository;
        private readonly ILogger<BodyService> _logger;

        public BodyService(ServiceBusClient serviceBusClient, IMapper mapper, IBodyRepository bodyRepository, ILogger<BodyService> logger)
        {
            _serviceBusClient = serviceBusClient;
            _mapper = mapper;
            _bodyRepository = bodyRepository;
            _logger = logger;
        }

        public async Task MapCardioEnvelopeAndSaveToDatabase(CardioResponseObject cardioResponseObject)
        {
            try
            {
                var vo2Record = new V02Record();
                _mapper.Map(cardioResponseObject, vo2Record);

                await _bodyRepository.AddV02Record(vo2Record);
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
                var weightRecord = new WeightRecord();
                _mapper.Map(weight, weightRecord);

                await _bodyRepository.AddWeightRecord(weightRecord);
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
