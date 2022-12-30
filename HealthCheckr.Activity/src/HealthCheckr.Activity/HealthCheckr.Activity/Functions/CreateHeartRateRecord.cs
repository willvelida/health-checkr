using HealthCheckr.Activity.Common.FitbitResponses;
using HealthCheckr.Activity.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HealthCheckr.Activity.Functions
{
    public class CreateHeartRateRecord
    {
        private readonly IActivityService _activityService;
        private readonly ILogger<CreateHeartRateRecord> _logger;

        public CreateHeartRateRecord(IActivityService activityService, ILogger<CreateHeartRateRecord> logger)
        {
            _activityService = activityService;
            _logger = logger;
        }

        [Function(nameof(CreateHeartRateRecord))]
        public async Task Run([ServiceBusTrigger("heartratequeue", Connection = "ServiceBusConnection")] string heartRateQueueItem)
        {
            try
            {
                var date = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                _logger.LogInformation($"Attempting to parse Heart Rate Response message for {date}");

                var heartRate = JsonConvert.DeserializeObject<HeartRateTimeSeriesResponse>(heartRateQueueItem);

                _logger.LogInformation($"Adding Heart Rate for {date} to database.");
                await _activityService.MapHeartRateEnvelopeAndSaveToDatabase(heartRate);
                _logger.LogInformation($"Heart Rate for {date} successfully persisted");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateHeartRateRecord)}: {ex.Message}");
                throw;
            }
        }
    }
}
