using AutoMapper;
using Azure.Messaging.ServiceBus;
using HealthCheckr.Sleep.Common.Envelopes;
using HealthCheckr.Sleep.Common.FitbitResponses;
using HealthCheckr.Sleep.Repository.Interfaces;
using HealthCheckr.Sleep.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HealthCheckr.Sleep.Services
{
    public class SleepService : ISleepService
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly IMapper _mapper;
        private readonly ISleepRepository _sleepRepository;
        private readonly ICosmosDbRepository _cosmosDbRepository;
        private readonly ILogger<SleepService> _logger;

        public SleepService(ServiceBusClient serviceBusClient, IMapper mapper, ISleepRepository sleepRepository, ICosmosDbRepository cosmosDbRepository, ILogger<SleepService> logger)
        {
            _serviceBusClient = serviceBusClient;
            _mapper = mapper;
            _sleepRepository = sleepRepository;
            _cosmosDbRepository = cosmosDbRepository;
            _logger = logger;
        }

        public async Task MapBreathingRecordAndSaveToDatabase(BreathingRateResponseObject breathingRateResponseObject)
        {
            try
            {
                var breathingRecord = new BreathingRateRecord();
                _mapper.Map(breathingRateResponseObject, breathingRecord);

                await _sleepRepository.AddBreathingRateRecord(breathingRecord);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapBreathingRecordAndSaveToDatabase)}: {ex.Message}");
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

        public async Task MapSp02RecordAndSaveToDatabase(Sp02ResponseObject sp02Response)
        {
            try
            {
                var sp02Record = new Sp02Record();
                _mapper.Map(sp02Response, sp02Record);

                await _sleepRepository.AddSp02Record(sp02Record);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapSp02RecordAndSaveToDatabase)}: {ex.Message}");
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
