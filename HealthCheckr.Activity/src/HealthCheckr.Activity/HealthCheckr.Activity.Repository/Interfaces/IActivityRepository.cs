using HealthCheckr.Activity.Common.Envelopes;

namespace HealthCheckr.Activity.Repository.Interfaces
{
    public interface IActivityRepository
    {
        Task AddActivityHeartRateZoneRecord(ActivityHeartRateZonesRecord activityHeartRateZonesRecord);
        Task AddActivityDistancesRecord(ActivityDistancesRecord activityDistancesRecord);
        Task AddActivitySummaryRecord(ActivitySummaryRecord activitySummaryRecord, int heartRateZoneId, int distanceId);
        Task AddActivityRecord(ActivityRecord activityRecord);
        Task<int> GetActivityHeartRateZoneId();
        Task<int> GetActivityDistanceId();
    }
}
