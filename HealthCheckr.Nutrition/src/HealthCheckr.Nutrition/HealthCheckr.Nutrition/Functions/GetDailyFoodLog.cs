using System;
using System.Security.Cryptography;
using HealthCheckr.Nutrition.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace HealthCheckr.Nutrition.Functions
{
    public class GetDailyFoodLog
    {
        private readonly IFitbitService _fitbitService;
        private readonly IFoodService _foodService;
        private readonly ILogger<GetDailyFoodLog> _logger;

        public GetDailyFoodLog(IFitbitService fitbitService, IFoodService foodService, ILogger<GetDailyFoodLog> logger)
        {
            _fitbitService= fitbitService;
            _foodService= foodService;
            _logger= logger;
        }

        [Function(nameof(GetDailyFoodLog))]
        public async Task Run([TimerTrigger("0 20 5 * * *")] TimerInfo myTimer)
        {
            try
            {
                _logger.LogInformation($"{nameof(GetDailyFoodLog)} executed at: {DateTime.Now}");
                var date = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                _logger.LogInformation($"Attempting to retrieve Food Log for {date}");
                var foodResponse = await _fitbitService.GetFoodLogs(date);

                _logger.LogInformation($"Mapping response to Food object and sending to queue.");
                await _foodService.SendFoodRecordToQueue(foodResponse);
                _logger.LogInformation($"Food Summary sent to queue.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetDailyFoodLog)}: {ex.Message}");
                throw;
            }
        }
    }
}
