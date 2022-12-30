using HealthCheckr.Activity.Common.Envelopes;

namespace HealthCheckr.Activity.Repository.Interfaces
{
    public interface ICosmosDbRepository
    {
        Task CreateActivityDocument(ActivityEnvelope activityEnvelope);
        Task CreateHeartRateDocument(HeartRateEnvelope heartRateEnvelope);
    }
}
