using HealthCheckr.Body.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace HealthCheckr.Body.Functions
{
    public class GetMonthlyWeightLogs
    {
        private readonly IFitbitService _fitbitService;
        private readonly IBodyService _bodyService;
        private readonly ILogger<GetMonthlyWeightLogs> _logger;

        public GetMonthlyWeightLogs(IFitbitService fitbitService, IBodyService bodyService, ILogger<GetMonthlyWeightLogs> logger)
        {
            _fitbitService = fitbitService;
            _bodyService = bodyService;
            _logger = logger;
        }

        [Function(nameof(GetMonthlyWeightLogs))]
        public async Task Run([TimerTrigger("0 0 6 1 * *")] TimerInfo myTimer)
        {
            try
            {
                _logger.LogInformation($"{nameof(GetMonthlyWeightLogs)} executed at: {DateTime.Now}");
                var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
                var endDate = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month));
                var startDateParameter = startDate.ToString("yyyy-MM-dd");
                var endDateParameter = endDate.ToString("yyyy-MM-dd");
                _logger.LogInformation($"Attempting to retrieve weight logs between {startDateParameter} and {endDateParameter}");

                var weightResponse = await _fitbitService.GetWeightResponse(startDateParameter, endDateParameter);

                foreach (var record in weightResponse.weight)
                {
                    await _bodyService.MapAndSendWeightRecordToQueue(record);
                }
                _logger.LogInformation($"Successfully mapped and sent {weightResponse.weight.Count} records to Service Bus.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetMonthlyWeightLogs)}: {ex.Message}");
                throw;
            }
        }
    }
}
