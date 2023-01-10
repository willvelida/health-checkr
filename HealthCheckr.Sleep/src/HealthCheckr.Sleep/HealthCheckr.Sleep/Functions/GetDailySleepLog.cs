using HealthCheckr.Sleep.Common;
using HealthCheckr.Sleep.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HealthCheckr.Sleep.Functions
{
    public class GetDailySleepLog
    {
        private readonly IFitbitService _fitbitService;
        private readonly ISleepService _sleepService;
        private readonly Settings _settings;
        private readonly ILogger<GetDailySleepLog> _logger;

        public GetDailySleepLog(IFitbitService fitbitService, ISleepService sleepService, IOptions<Settings> options, ILogger<GetDailySleepLog> logger)
        {
            _fitbitService = fitbitService;
            _sleepService = sleepService;
            _settings = options.Value;
            _logger = logger;
        }

        [Function(nameof(GetDailySleepLog))]
        public async Task Run([TimerTrigger("0 0 5 * * *")] TimerInfo myTimer)
        {
            try
            {
                _logger.LogInformation($"{nameof(GetDailySleepLog)} executed at: {DateTime.Now}");
                var dateParameter = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                _logger.LogInformation($"Attempting to retrieve Sleep Log for {dateParameter}");
                var sleepResponse = await _fitbitService.GetSleepResponse(dateParameter);


                _logger.LogInformation("Sending mapped Sleep log to service bus");
                await _sleepService.SendRecordToQueue(sleepResponse, _settings.SleepQueueName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetDailySleepLog)}: {ex.Message}");
                throw;
            }
        }
    }
}
