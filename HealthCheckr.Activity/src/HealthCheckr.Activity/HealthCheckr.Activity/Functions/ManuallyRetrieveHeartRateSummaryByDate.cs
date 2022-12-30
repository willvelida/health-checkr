using HealthCheckr.Activity.Common;
using HealthCheckr.Activity.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HealthCheckr.Activity.Functions
{
    public class ManuallyRetrieveHeartRateSummaryByDate
    {
        private readonly IFitbitService _fitbitService;
        private readonly IActivityService _activityService;
        private readonly ILogger<ManuallyRetrieveHeartRateSummaryByDate> _logger;
        private readonly Settings _settings;

        public ManuallyRetrieveHeartRateSummaryByDate(IFitbitService fitbitService,
            IActivityService activityService,
            ILogger<ManuallyRetrieveHeartRateSummaryByDate> logger,
            IOptions<Settings> options)
        {
            _settings = options.Value;
            _fitbitService = fitbitService;
            _activityService = activityService;
            _logger = logger;
        }

        [Function(nameof(ManuallyRetrieveHeartRateSummaryByDate))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            IActionResult result;

            try
            {
                _logger.LogInformation($"{nameof(ManuallyRetrieveHeartRateSummaryByDate)} executed at: {DateTime.Now}");
                var date = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

                _logger.LogInformation($"Attempting to retrieve Heart Rate Series Data for {date}");
                var heartRateTimeSeriesResponse = await _fitbitService.GetHeartRateTimeSeriesByDate(date);

                _logger.LogInformation($"Mapping response to Heart Rate object and Sending to queue.");
                await _activityService.SendRecordToQueue(heartRateTimeSeriesResponse, _settings.HeartRateQueueName);
                _logger.LogInformation($"Heart Rate Series Data sent to queue.");

                result = new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(ManuallyRetrieveHeartRateSummaryByDate)}: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }
    }
}
