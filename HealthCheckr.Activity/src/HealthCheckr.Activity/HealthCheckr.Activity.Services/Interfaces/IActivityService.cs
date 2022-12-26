using HealthCheckr.Activity.Common.FitbitResponses;
using env = HealthCheckr.Activity.Common.Envelopes;

namespace HealthCheckr.Activity.Services.Interfaces
{
    public interface IActivityService
    {
        Task MapAndSendActivityRecordToQueue(string date, ActivityResponse activityResponse);
        Task MapActivityEnvelopeAndSaveToDatabase(env.Activity activity);
        Task<List<env.ActivityEnvelope>> GetAllActivityRecords();
        Task<env.ActivityEnvelope> GetActivityRecordByDate(string activityDate);
    }
}
