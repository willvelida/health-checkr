using HealthCheckr.Sleep.Common.Envelopes;
using HealthCheckr.Sleep.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace HealthCheckr.Sleep.Repository
{
    public class SleepRepository : ISleepRepository
    {
        private readonly SleepContext _context;
        private readonly ILogger<SleepRepository> _logger;

        public SleepRepository(SleepContext context, ILogger<SleepRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddBreathingRateRecord(BreathingRateRecord rateRecord)
        {
            try
            {
                _logger.LogInformation($"Attempting to persist Breathing Rate record for date: {rateRecord.Date}");
                _context.BreathingRate.Add(rateRecord);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Breathing Rate record for date: {rateRecord.Date} successfully persisted");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(AddBreathingRateRecord)}: {ex.Message}");
                throw;
            }
        }

        public async Task AddSleepAndSleepSummaryRecord(SleepSummaryRecord sleepSummaryRecord, SleepRecord sleepRecord)
        {
            try
            {
                _logger.LogInformation($"Persisting Sleep and Sleep Summary record. Begin transaction.");
                using var transaction = _context.Database.BeginTransaction();

                _logger.LogInformation($"Attempting to persist Sleep Summary record for {sleepSummaryRecord.Date}");
                _context.SleepSummary.Add(sleepSummaryRecord);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Sleep Summary record for {sleepSummaryRecord.Date} successfully persisted");

                _logger.LogInformation($"Getting Sleep Summary Id");
                var sleepSummaryId = _context.SleepSummary
                    .OrderByDescending(p => p.Id)
                    .Select(p => p.Id)
                    .FirstOrDefault();
                _logger.LogInformation($"Sleep Summary Id: {sleepSummaryId}. Setting it in Sleep record");
                sleepRecord.SleepSummaryId = sleepSummaryId;

                _logger.LogInformation($"Attempting to persist Sleep record for {sleepRecord.Date}");
                _context.Sleep.Add(sleepRecord);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Sleep record for {sleepRecord.Date} successfully persisted");

                _logger.LogInformation($"Committing transaction");
                await transaction.CommitAsync();
                _logger.LogInformation($"Transaction committed");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(AddSleepAndSleepSummaryRecord)}: {ex.Message}");
                throw;
            }
        }

        public async Task AddSp02Record(Sp02Record sp02Record)
        {
            try
            {
                _logger.LogInformation($"Attempting to persist SP02 record for date: {sp02Record.Date}");
                _context.Sp02.Add(sp02Record);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"SP02 record for date: {sp02Record.Date} successfully persisted");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(AddSp02Record)}: {ex.Message}");
                throw;
            }
        }
    }
}
