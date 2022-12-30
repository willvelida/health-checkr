using HealthCheckr.Activity.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace HealthCheckr.Activity.Functions
{
    public class ManuallyRetrieveActivitySummaryByDate
    {
        private readonly IFitbitService _fitbitService;
        private readonly IActivityService _activityService;
        private readonly ILogger<ManuallyRetrieveActivitySummaryByDate> _logger;

        public ManuallyRetrieveActivitySummaryByDate(IFitbitService fitbitService,
            IActivityService activityService,
            ILogger<ManuallyRetrieveActivitySummaryByDate> logger)
        {
            _fitbitService = fitbitService;
            _activityService = activityService;
            _logger = logger;
        }

        [Function(nameof(ManuallyRetrieveActivitySummaryByDate))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            IActionResult result;

            try
            {
                _logger.LogInformation($"{nameof(GetActivitySummaryByDate)} executed at: {DateTime.Now}");
                var date = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

                _logger.LogInformation($"Attempting to retrieve Daily Activity Summary for {date}");
                var activityResponse = await _fitbitService.GetActivityResponse(date);

                _logger.LogInformation($"Mapping response to Activity object and Sending to queue.");
                await _activityService.MapAndSendActivityRecordToQueue(activityResponse);
                _logger.LogInformation($"Activity Summary sent to queue.");

                result = new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetActivitySummaryByDate)}: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }
    }
}
