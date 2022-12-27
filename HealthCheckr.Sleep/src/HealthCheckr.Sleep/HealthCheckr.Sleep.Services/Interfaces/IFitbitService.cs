using HealthCheckr.Sleep.Common.FitbitResponses;

namespace HealthCheckr.Sleep.Services.Interfaces
{
    public interface IFitbitService
    {
        Task<SleepResponseObject> GetSleepResponse(string date);
    }
}
