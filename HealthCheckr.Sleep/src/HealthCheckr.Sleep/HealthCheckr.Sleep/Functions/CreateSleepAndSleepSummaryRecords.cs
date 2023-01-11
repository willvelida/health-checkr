using HealthCheckr.Sleep.Common.Envelopes;
using HealthCheckr.Sleep.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace HealthCheckr.Sleep.Functions
{
    public class CreateSleepAndSleepSummaryRecords
    {
        private readonly ISleepService _sleepService;
        private readonly ILogger<CreateSleepAndSleepSummaryRecords> _logger;

        public CreateSleepAndSleepSummaryRecords(ISleepService sleepService, ILogger<CreateSleepAndSleepSummaryRecords> logger)
        {
            _sleepService = sleepService;
            _logger = logger;
        }

        [Function(nameof(CreateSleepAndSleepSummaryRecords))]
        public async Task Run([CosmosDBTrigger(
            databaseName: "HealthCheckrDB",
            containerName: "Sleep",
            Connection = "CosmosDbConnection",
            LeaseContainerName = "leases",
            LeaseContainerPrefix = "Sleep")] IReadOnlyList<SleepEnvelope> sleepEnvelopes)
        {
            try
            {
                if (sleepEnvelopes != null && sleepEnvelopes.Count > 0)
                {
                    foreach (var sleepEnvelope in sleepEnvelopes)
                    {
                        _logger.LogInformation($"Attempting to denormalize Sleep Envelope document for {sleepEnvelope.Date}");
                        await _sleepService.SaveSleepAndSleepSummaryRecord(sleepEnvelope);
                        _logger.LogInformation($"Records for {sleepEnvelope.Date} saved");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateSleepAndSleepSummaryRecords)}: {ex.Message}");
                throw;
            }
        }
    }
}
