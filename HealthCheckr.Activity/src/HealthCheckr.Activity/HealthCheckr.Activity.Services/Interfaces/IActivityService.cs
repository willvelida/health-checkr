using HealthCheckr.Activity.Common.Envelopes;
using HealthCheckr.Activity.Common.FitbitResponses;

namespace HealthCheckr.Activity.Services.Interfaces
{
    public interface IActivityService
    {
        Task MapActivityEnvelopeAndSaveToDatabase(string date, ActivityResponse activityResponse);
        Task MapAndSaveActivityHeartRateRecord(ActivityEnvelope activityEnvelope);
        Task MapAndSaveActivityDistanceRecord(ActivityEnvelope activityEnvelope);
        Task MapAndSaveActivitySummaryRecord(ActivityEnvelope activityEnvelope);
        Task MapAndSaveActivityRecords(ActivityEnvelope activityEnvelope);
        Task SendRecordToQueue<T>(T record, string queueName);
    }
}
