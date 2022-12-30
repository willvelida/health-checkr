using HealthCheckr.Body.Common.FitbitResponses;
using HealthCheckr.Body.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HealthCheckr.Body.Functions
{
    public class CreateWeightRecord
    {
        private readonly IBodyService _bodyService;
        private readonly ILogger<GetMonthlyWeightLogs> _logger;

        public CreateWeightRecord(IBodyService bodyService, ILogger<GetMonthlyWeightLogs> logger)
        {
            _bodyService = bodyService;
            _logger = logger;
        }

        [Function(nameof(CreateWeightRecord))]
        public async Task Run([ServiceBusTrigger("bodyqueue", Connection = "ServiceBusConnection")] string bodyQueueItem)
        {
            try
            {
                var weight = JsonConvert.DeserializeObject<Weight>(bodyQueueItem);
                await _bodyService.MapWeightEnvelopeAndSaveToDatabase(weight);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateWeightRecord)}: {ex.Message}");
                throw;
            }
        }
    }
}
