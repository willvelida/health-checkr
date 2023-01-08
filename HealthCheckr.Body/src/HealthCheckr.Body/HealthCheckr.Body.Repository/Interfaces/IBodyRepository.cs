using HealthCheckr.Body.Common.Envelopes;

namespace HealthCheckr.Body.Repository.Interfaces
{
    public interface IBodyRepository
    {
        Task AddV02Record(V02Record v02Record);
        Task AddWeightRecord(WeightRecord weightRecord);
    }
}
