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
    public class ManuallyRetrieveDailySp02Log
    {
        private readonly IFitbitService _fitbitService;
        private readonly ISleepService _sleepService;
        private readonly Settings _settings;
        private readonly ILogger<ManuallyRetrieveDailySp02Log> _logger;

        public ManuallyRetrieveDailySp02Log(IFitbitService fitbitService, ISleepService sleepService, IOptions<Settings> options, ILogger<ManuallyRetrieveDailySp02Log> logger)
        {
            _fitbitService = fitbitService;
            _sleepService = sleepService;
            _settings = options.Value;
            _logger = logger;
        }

        [Function(nameof(ManuallyRetrieveDailySp02Log))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            IActionResult result;

            try
            {
                _logger.LogInformation($"{nameof(ManuallyRetrieveDailySp02Log)} executed at: {DateTime.Now}");
                var dateParameter = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                _logger.LogInformation($"Attempting to retrieve Sp02 Log for {dateParameter}");
                var sp02Response = await _fitbitService.GetSp02Response(dateParameter);


                _logger.LogInformation("Sending Sp02 log to service bus");
                await _sleepService.SendRecordToQueue(sp02Response, _settings.Sp02QueueName);

                result = new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(ManuallyRetrieveDailySp02Log)}: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }
    }
}
