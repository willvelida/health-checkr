using HealthCheckr.Activity.Common;
using HealthCheckr.Activity.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HealthCheckr.Activity.Functions
{
    public class GetHeartRateSeriesByDate
    {
        private readonly IFitbitService _fitbitService;
        private readonly IActivityService _activityService;
        private readonly ILogger<GetHeartRateSeriesByDate> _logger;
        private readonly Settings _settings;

        public GetHeartRateSeriesByDate(IFitbitService fitbitService,
            IActivityService activityService,
            ILogger<GetHeartRateSeriesByDate> logger,
            IOptions<Settings> options)
        {
            _settings = options.Value;
            _fitbitService = fitbitService;
            _activityService = activityService;
            _logger = logger;
        }

        [Function(nameof(GetHeartRateSeriesByDate))]
        public async Task Run([TimerTrigger("0 25 5 * * *")] TimerInfo myTimer)
        {
            try
            {
                _logger.LogInformation($"{nameof(GetHeartRateSeriesByDate)} executed at: {DateTime.Now}");
                var date = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

                _logger.LogInformation($"Attempting to retrieve Heart Rate Series Data for {date}");
                var heartRateTimeSeriesResponse = await _fitbitService.GetHeartRateTimeSeriesByDate(date);

                _logger.LogInformation($"Mapping response to Heart Rate object and Sending to queue.");
                await _activityService.SendRecordToQueue(heartRateTimeSeriesResponse, _settings.HeartRateQueueName);
                _logger.LogInformation($"Heart Rate Series Data sent to queue.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetHeartRateSeriesByDate)}: {ex.Message}");
                throw;
            }
        }
    }
}
