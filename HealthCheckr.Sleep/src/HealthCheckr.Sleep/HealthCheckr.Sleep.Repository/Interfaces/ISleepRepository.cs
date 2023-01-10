using HealthCheckr.Sleep.Common.Envelopes;

namespace HealthCheckr.Sleep.Repository.Interfaces
{
    public interface ISleepRepository
    {
        Task AddSp02Record(Sp02Record sp02Record);
        Task AddBreathingRateRecord(BreathingRateRecord rateRecord);
    }
}
