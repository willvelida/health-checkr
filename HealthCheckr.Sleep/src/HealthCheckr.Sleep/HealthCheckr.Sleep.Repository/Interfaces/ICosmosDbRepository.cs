using HealthCheckr.Sleep.Common.Envelopes;

namespace HealthCheckr.Sleep.Repository.Interfaces
{
    public interface ICosmosDbRepository
    {
        Task CreateSleepDocument(SleepEnvelope sleepEnvelope);
        Task CreateSp02Document(Sp02Envelope sp02Envelope);
    }
}
