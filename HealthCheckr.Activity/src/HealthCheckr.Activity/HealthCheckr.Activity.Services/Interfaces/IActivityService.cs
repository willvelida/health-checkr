using HealthCheckr.Activity.Common.FitbitResponses;

namespace HealthCheckr.Activity.Services.Interfaces
{
    public interface IActivityService
    {
        Task MapActivityEnvelopeAndSaveToDatabase(string date, ActivityResponse activityResponse);
        Task MapHeartRateEnvelopeAndSaveToDatabase(HeartRateTimeSeriesResponse heartRateTimeSeriesResponse);
        Task SendRecordToQueue<T>(T record, string queueName);
    }
}
