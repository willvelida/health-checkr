using HealthCheckr.Activity.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using env = HealthCheckr.Activity.Common.Envelopes;

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

        [Function("CreateActivityRecord")]
        public async Task RunAsync([ServiceBusTrigger("activityqueue", Connection = "ServiceBusConnection")] string activityQueueItem)
        {
            try
            {
                var activity = JsonConvert.DeserializeObject<env.Activity>(activityQueueItem);
                await _activityService.MapActivityEnvelopeAndSaveToDatabase(activity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateActivityRecord)}: {ex.Message}");
                throw;
            }
        }
    }
}
