using HealthCheckr.Activity.Common.FitbitResponses;
using HealthCheckr.Activity.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HealthCheckr.Activity.Functions
{
    public class CreateActivityRecord
    {
        private readonly IActivityService _activityService;
        private readonly ILogger<CreateActivityRecord> _logger;

        public CreateActivityRecord(IActivityService activityService, ILogger<CreateActivityRecord> logger)
        {
            _activityService = activityService;
            _logger = logger;
        }

        [Function(nameof(CreateActivityRecord))]
        public async Task RunAsync([ServiceBusTrigger("activityqueue", Connection = "ServiceBusConnection")] string activityQueueItem)
        {
            try
            {
                var date = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                _logger.LogInformation($"Attempting to parse Activity Response message for {date}");

                var activity = JsonConvert.DeserializeObject<ActivityResponse>(activityQueueItem);

                _logger.LogInformation($"Adding activity for {date} to database.");
                await _activityService.MapActivityEnvelopeAndSaveToDatabase(date, activity);
                _logger.LogInformation($"Activity for {date} successfully persisted");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateActivityRecord)}: {ex.Message}");
                throw;
            }
        }
    }
}
