using HealthCheckr.Sleep.Common.Envelopes;

namespace HealthCheckr.Sleep.Repository.Interfaces
{
    public interface ICosmosDbRepository
    {
        Task CreateSleepDocument(SleepEnvelope sleepEnvelope);
        Task<List<SleepEnvelope>> GetSleepEnvelopes();
        Task<SleepEnvelope> GetSleepEnvelopeByDate(string sleepDate);
    }
}
