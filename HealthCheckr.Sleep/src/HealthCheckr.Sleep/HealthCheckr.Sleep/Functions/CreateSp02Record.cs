using HealthCheckr.Sleep.Common.FitbitResponses;
using HealthCheckr.Sleep.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HealthCheckr.Sleep.Functions
{
    public class CreateSp02Record
    {
        private readonly ISleepService _sleepService;
        private readonly ILogger<CreateSp02Record> _logger;

        public CreateSp02Record(ISleepService sleepService, ILogger<CreateSp02Record> logger)
        {
            _sleepService = sleepService;
            _logger = logger;
        }

        [Function(nameof(CreateSp02Record))]
        public async Task Run([ServiceBusTrigger("sp02queue", Connection = "ServiceBusConnection")] string sp02QueueItem)
        {
            try
            {
                _logger.LogInformation("Attempting to parse incoming sp02 message");
                var sp02 = JsonConvert.DeserializeObject<Sp02ResponseObject>(sp02QueueItem);
                _logger.LogInformation($"Parse successful! Persisting sp02 for {sp02.dateTime} to database");

                await _sleepService.MapSp02RecordAndSaveToDatabase(sp02);
                _logger.LogInformation($"Sp02 record for {sp02.dateTime} saved to database.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateSleepRecord)}: {ex.Message}");
                throw;
            }
        }
    }
}
