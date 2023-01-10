using HealthCheckr.Sleep.Common;
using HealthCheckr.Sleep.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HealthCheckr.Sleep.Functions
{
    public class GetDailySp02Log
    {
        private readonly IFitbitService _fitbitService;
        private readonly ISleepService _sleepService;
        private readonly Settings _settings;
        private readonly ILogger<GetDailySp02Log> _logger;

        public GetDailySp02Log(IFitbitService fitbitService, ISleepService sleepService, IOptions<Settings> options, ILogger<GetDailySp02Log> logger)
        {
            _fitbitService = fitbitService;
            _sleepService = sleepService;
            _settings = options.Value;
            _logger = logger;
        }

        [Function(nameof(GetDailySp02Log))]
        public async Task Run([TimerTrigger("0 10 5 * * *")] TimerInfo myTimer)
        {
            try
            {
                _logger.LogInformation($"{nameof(GetDailySleepLog)} executed at: {DateTime.Now}");
                var dateParameter = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                _logger.LogInformation($"Attempting to retrieve Sp02 Log for {dateParameter}");
                var sp02Response = await _fitbitService.GetSp02Response(dateParameter);


                _logger.LogInformation("Sending Sp02 log to service bus");
                await _sleepService.SendRecordToQueue(sp02Response, _settings.Sp02QueueName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetDailySp02Log)}: {ex.Message}");
                throw;
            }
        }
    }
}
