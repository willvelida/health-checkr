using HealthCheckr.Body.Common.Envelopes;
using HealthCheckr.Body.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace HealthCheckr.Body.Repository
{
    public class BodyRepository : IBodyRepository
    {
        private readonly BodyContext _context;
        private readonly ILogger<BodyRepository> _logger;

        public BodyRepository(BodyContext context, ILogger<BodyRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddV02Record(V02Record v02Record)
        {
            try
            {
                _logger.LogInformation($"Attempting to persist V02 record for date: {v02Record.Date}");
                _context.V02.Add(v02Record);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"V02 record for date: {v02Record.Date} successfully added");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(AddV02Record)}: {ex.Message}");
                throw;
            }
        }

        public async Task AddWeightRecord(WeightRecord weightRecord)
        {
            try
            {
                _logger.LogInformation($"Attempting to persist Weight record for date: {weightRecord.Date}");
                _context.Weight.Add(weightRecord);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Weight record for date: {weightRecord.Date} successfully added");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(AddWeightRecord)}: {ex.Message}");
                throw;
            }
        }
    }
}
