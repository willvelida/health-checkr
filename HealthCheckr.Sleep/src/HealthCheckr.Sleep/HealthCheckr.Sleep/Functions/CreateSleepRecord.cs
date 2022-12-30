using HealthCheckr.Sleep.Common.FitbitResponses;
using HealthCheckr.Sleep.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HealthCheckr.Sleep.Functions
{
    public class CreateSleepRecord
    {
        private readonly ISleepService _sleepService;
        private readonly ILogger<CreateSleepRecord> _logger;

        public CreateSleepRecord(ISleepService sleepService, ILogger<CreateSleepRecord> logger)
        {
            _sleepService = sleepService;
            _logger = logger;
        }

        [Function(nameof(CreateSleepRecord))]
        public async Task Run([ServiceBusTrigger("sleepqueue", Connection = "ServiceBusConnection")] string sleepQueueItem)
        {
            try
            {
                _logger.LogInformation("Attempting to parse incoming sleep message");
                var sleep = JsonConvert.DeserializeObject<SleepResponseObject>(sleepQueueItem);
                _logger.LogInformation($"Parse successful! Persisting sleep for ${sleep.sleep[0].dateOfSleep} to database!");

                await _sleepService.MapSleepEnvelopeAndSaveToDatabase(sleep);
                _logger.LogInformation($"Sp02 record for {sleep.sleep[0].dateOfSleep} saved to database.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateSleepRecord)}: {ex.Message}");
                throw;
            }
        }
    }
}
