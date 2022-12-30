using HealthCheckr.Body.Common.Envelopes;

namespace HealthCheckr.Body.Repository.Interfaces
{
    public interface ICosmosDbRepository
    {
        Task CreateWeightDocument(WeightEnvelope weightEnvelope);
        Task CreateV02MaxDocument(CardioEnvelope cardioEnvelope);
    }
}
