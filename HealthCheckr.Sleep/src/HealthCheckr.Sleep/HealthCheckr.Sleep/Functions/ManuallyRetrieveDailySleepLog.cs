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
    public class ManuallyRetrieveDailySleepLog
    {
        private readonly IFitbitService _fitbitService;
        private readonly ISleepService _sleepService;
        private readonly Settings _settings;
        private readonly ILogger<ManuallyRetrieveDailySleepLog> _logger;

        public ManuallyRetrieveDailySleepLog(IFitbitService fitbitService, ISleepService sleepService, IOptions<Settings> options, ILogger<ManuallyRetrieveDailySleepLog> logger)
        {
            _fitbitService = fitbitService;
            _sleepService = sleepService;
            _settings = options.Value;
            _logger = logger;
        }

        [Function(nameof(ManuallyRetrieveDailySleepLog))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            IActionResult result;

            try
            {
                _logger.LogInformation($"{nameof(ManuallyRetrieveDailySleepLog)} executed at: {DateTime.Now}");
                var dateParameter = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                _logger.LogInformation($"Attempting to retrieve Sleep Log for {dateParameter}");
                var sleepResponse = await _fitbitService.GetSleepResponse(dateParameter);


                _logger.LogInformation("Sending mapped Sleep log to service bus");
                await _sleepService.SendRecordToQueue(sleepResponse, _settings.SleepQueueName);

                result = new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(ManuallyRetrieveDailySleepLog)}: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }
    }
}
