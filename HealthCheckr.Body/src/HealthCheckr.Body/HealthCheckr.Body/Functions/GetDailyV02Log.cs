using HealthCheckr.Body.Common;
using HealthCheckr.Body.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HealthCheckr.Body.Functions
{
    public class GetDailyV02Log
    {
        private readonly IFitbitService _fitbitService;
        private readonly IBodyService _bodyService;
        private readonly ILogger<GetDailyV02Log> _logger;
        private readonly Settings _settings;

        public GetDailyV02Log(IFitbitService fitbitService, IBodyService bodyService, ILogger<GetDailyV02Log> logger, IOptions<Settings> options)
        {
            _settings = options.Value;
            _fitbitService = fitbitService;
            _bodyService = bodyService;
            _logger = logger;
        }

        [Function(nameof(GetDailyV02Log))]
        public async Task Run([TimerTrigger("0 15 4 * * *")] TimerInfo myTimer)
        {
            try
            {
                _logger.LogInformation($"{nameof(GetDailyV02Log)} executed at: {DateTime.Now}");
                var date = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

                _logger.LogInformation($"Attempting to retrieve Daily V02 Summary for {date}");
                var cardioResponse = await _fitbitService.GetV02MaxSummary(date);

                _logger.LogInformation($"Mapping response to v02 object and Sending to queue.");
                await _bodyService.SendRecordToQueue(cardioResponse, _settings.V02QueueName);
                _logger.LogInformation($"V02 Summary sent to queue.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetDailyV02Log)}: {ex.Message}");
                throw;
            }
        }
    }
}
