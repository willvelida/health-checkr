using HealthCheckr.Sleep.Common;
using HealthCheckr.Sleep.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HealthCheckr.Sleep.Functions
{
    public class ManuallyRetrieveDailyBreathingRateLog
    {
        private readonly IFitbitService _fitbitService;
        private readonly ISleepService _sleepService;
        private readonly Settings _settings;
        private readonly ILogger<ManuallyRetrieveDailyBreathingRateLog> _logger;

        public ManuallyRetrieveDailyBreathingRateLog(IFitbitService fitbitService, ISleepService sleepService, IOptions<Settings> options, ILogger<ManuallyRetrieveDailyBreathingRateLog> logger)
        {
            _fitbitService = fitbitService;
            _sleepService = sleepService;
            _settings = options.Value;
            _logger = logger;
        }

        [Function(nameof(ManuallyRetrieveDailyBreathingRateLog))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            IActionResult result;

            try
            {
                _logger.LogInformation($"{nameof(GetDailyBreathingRateLog)} executed at: {DateTime.Now}");
                var dateParameter = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                _logger.LogInformation($"Attempting to retrieve Breathing Rate Log for {dateParameter}");
                var breathingRateResponse = await _fitbitService.GetBreathingRateResponse(dateParameter);


                _logger.LogInformation("Sending Breathing Rate Log to service bus");
                await _sleepService.SendRecordToQueue(breathingRateResponse, _settings.BreathingRateQueueName);

                result = new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(ManuallyRetrieveDailyBreathingRateLog)}: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }
    }
}
