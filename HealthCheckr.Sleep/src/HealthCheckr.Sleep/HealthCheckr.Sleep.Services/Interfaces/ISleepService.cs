using HealthCheckr.Sleep.Common.FitbitResponses;
using env = HealthCheckr.Sleep.Common.Envelopes;

namespace HealthCheckr.Sleep.Services.Interfaces
{
    public interface ISleepService
    {
        Task MapAndSendSleepRecordToQueue(SleepResponseObject sleepResponse);
        Task MapSleepEnvelopeAndSaveToDatabase(SleepResponseObject sleepResponse);
        Task<List<env.SleepEnvelope>> GetAllSleepRecords();
        Task<env.SleepEnvelope> GetSleepRecordByDate(string sleepDate);
    }
}
