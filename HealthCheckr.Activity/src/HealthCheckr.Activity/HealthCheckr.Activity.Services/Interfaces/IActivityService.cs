using HealthCheckr.Activity.Common.FitbitResponses;

namespace HealthCheckr.Activity.Services.Interfaces
{
    public interface IActivityService
    {
        Task MapAndSendActivityRecordToQueue(ActivityResponse activityResponse);
        Task MapActivityEnvelopeAndSaveToDatabase(string date, ActivityResponse activityResponse);
    }
}
