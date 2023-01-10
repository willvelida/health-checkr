using HealthCheckr.Sleep.Common.FitbitResponses;
using HealthCheckr.Sleep.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HealthCheckr.Sleep.Functions
{
    public class CreateBreathingRateRecord
    {
        private readonly ISleepService _sleepService;
        private readonly ILogger<CreateBreathingRateRecord> _logger;

        public CreateBreathingRateRecord(ISleepService sleepService, ILogger<CreateBreathingRateRecord> logger)
        {
            _sleepService = sleepService;
            _logger = logger;
        }

        [Function(nameof(CreateBreathingRateRecord))]
        public async Task Run([ServiceBusTrigger("breathingratequeue", Connection = "ServiceBusConnection")] string brQueueItem)
        {
            try
            {
                _logger.LogInformation("Attempting to parse incoming Breathing Rate message");
                var breathingRate = JsonConvert.DeserializeObject<BreathingRateResponseObject>(brQueueItem);
                _logger.LogInformation($"Parse successful! Persisting Breathing Rate record for ${breathingRate.br[0].dateTime} to database!");

                await _sleepService.MapBreathingRecordAndSaveToDatabase(breathingRate);
                _logger.LogInformation($"Sp02 record for {breathingRate.br[0].dateTime} saved to database.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateBreathingRateRecord)}: {ex.Message}");
                throw;
            }
        }
    }
}
