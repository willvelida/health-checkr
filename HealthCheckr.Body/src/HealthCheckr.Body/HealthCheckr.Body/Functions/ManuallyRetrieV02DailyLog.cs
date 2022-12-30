using HealthCheckr.Body.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace HealthCheckr.Body.Functions
{
    public class ManuallyRetrieV02DailyLog
    {
        private readonly IFitbitService _fitbitService;
        private readonly IBodyService _bodyService;
        private readonly ILogger<ManuallyRetrieV02DailyLog> _logger;

        public ManuallyRetrieV02DailyLog(IFitbitService fitbitService, IBodyService bodyService, ILogger<ManuallyRetrieV02DailyLog> logger)
        {
            _fitbitService = fitbitService;
            _bodyService = bodyService;
            _logger = logger;
        }

        [Function(nameof(ManuallyRetrieV02DailyLog))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            IActionResult result;

            try
            {
                _logger.LogInformation($"{nameof(GetDailyV02Log)} executed at: {DateTime.Now}");
                var date = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

                _logger.LogInformation($"Attempting to retrieve Daily V02 Summary for {date}");
                var cardioResponse = await _fitbitService.GetV02MaxSummary(date);

                _logger.LogInformation($"Mapping response to v02 object and Sending to queue.");
                await _bodyService.SendCardioResponseObjectToQueue(cardioResponse);
                _logger.LogInformation($"V02 Summary sent to queue.");

                result = new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(ManuallyRetrieV02DailyLog)}: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }
    }
}
