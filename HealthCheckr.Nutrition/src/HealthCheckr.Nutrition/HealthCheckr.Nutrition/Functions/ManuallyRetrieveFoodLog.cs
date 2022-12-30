using System.Net;
using HealthCheckr.Nutrition.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace HealthCheckr.Nutrition.Functions
{
    public class ManuallyRetrieveFoodLog
    {
        private readonly IFitbitService _fitbitService;
        private readonly IFoodService _foodService;
        private readonly ILogger<GetDailyFoodLog> _logger;

        public ManuallyRetrieveFoodLog(IFitbitService fitbitService, IFoodService foodService, ILogger<GetDailyFoodLog> logger)
        {
            _fitbitService = fitbitService;
            _foodService = foodService;
            _logger = logger;
        }

        [Function(nameof(ManuallyRetrieveFoodLog))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            IActionResult result;

            try
            {
                _logger.LogInformation($"{nameof(ManuallyRetrieveFoodLog)} executed at: {DateTime.Now}");
                var date = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                _logger.LogInformation($"Attempting to retrieve Food Log for {date}");
                var foodResponse = await _fitbitService.GetFoodLogs(date);

                _logger.LogInformation($"Mapping response to Food object and sending to queue.");
                await _foodService.SendFoodRecordToQueue(foodResponse);
                _logger.LogInformation($"Food Summary sent to queue.");

                result = new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(ManuallyRetrieveFoodLog)}: {ex.Message}");
                throw;
            }

            return result;
        }
    }
}
