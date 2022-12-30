using HealthCheckr.Body.Common.FitbitResponses;
using HealthCheckr.Body.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HealthCheckr.Body.Functions
{
    public class CreateV02Record
    {
        private readonly IBodyService _bodyService;
        private readonly ILogger<CreateV02Record> _logger;

        public CreateV02Record(IBodyService bodyService, ILogger<CreateV02Record> logger)
        {
            _bodyService = bodyService;
            _logger = logger;
        }

        [Function(nameof(CreateV02Record))]
        public async Task Run([ServiceBusTrigger("v02queue", Connection = "ServiceBusConnection")] string v02QueueItem)
        {
            try
            {
                _logger.LogInformation("Parsing incoming v02 message");
                var cardio = JsonConvert.DeserializeObject<CardioResponseObject>(v02QueueItem);
                _logger.LogInformation($"Parse successful! Persisting V02 record for {cardio.cardioScore[0].dateTime}");

                await _bodyService.MapCardioEnvelopeAndSaveToDatabase(cardio);
                _logger.LogInformation($"V02 Record for {cardio.cardioScore[0].dateTime} successfully saved to database.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateWeightRecord)}: {ex.Message}");
                throw;
            }
        }
    }
}
