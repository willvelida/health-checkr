using HealthCheckr.Sleep.Common;
using HealthCheckr.Sleep.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HealthCheckr.Sleep.Functions
{
    public class GetDailyBreathingRateLog
    {
        private readonly IFitbitService _fitbitService;
        private readonly ISleepService _sleepService;
        private readonly Settings _settings;
        private readonly ILogger<GetDailyBreathingRateLog> _logger;

        public GetDailyBreathingRateLog(IFitbitService fitbitService, ISleepService sleepService, IOptions<Settings> options, ILogger<GetDailyBreathingRateLog> logger)
        {
            _fitbitService = fitbitService;
            _sleepService = sleepService;
            _settings = options.Value;
            _logger = logger;
        }

        [Function(nameof(GetDailyBreathingRateLog))]
        public async Task Run([TimerTrigger("0 0 7 * * *")] TimerInfo myTimer)
        {
            try
            {
                _logger.LogInformation($"{nameof(GetDailyBreathingRateLog)} executed at: {DateTime.Now}");
                var dateParameter = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                _logger.LogInformation($"Attempting to retrieve Breathing Rate Log for {dateParameter}");
                var breathingRateResponse = await _fitbitService.GetBreathingRateResponse(dateParameter);


                _logger.LogInformation("Sending Breathing Rate Log to service bus");
                await _sleepService.SendRecordToQueue(breathingRateResponse, _settings.BreathingRateQueueName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetDailyBreathingRateLog)}: {ex.Message}");
                throw;
            }
        }
    }
}
