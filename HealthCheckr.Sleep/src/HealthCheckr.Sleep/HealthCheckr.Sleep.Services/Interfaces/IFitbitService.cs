using HealthCheckr.Sleep.Common.FitbitResponses;

namespace HealthCheckr.Sleep.Services.Interfaces
{
    public interface IFitbitService
    {
        Task<SleepResponseObject> GetSleepResponse(string date);
        Task<Sp02ResponseObject> GetSp02Response(string date);
        Task<BreathingRateResponseObject> GetBreathingRateResponse(string date);
    }
}
