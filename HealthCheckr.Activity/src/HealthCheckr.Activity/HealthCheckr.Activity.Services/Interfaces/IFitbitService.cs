using HealthCheckr.Activity.Common.FitbitResponses;

namespace HealthCheckr.Activity.Services.Interfaces
{
    public interface IFitbitService
    {
        Task<ActivityResponse> GetActivityResponse(string date);
    }
}
