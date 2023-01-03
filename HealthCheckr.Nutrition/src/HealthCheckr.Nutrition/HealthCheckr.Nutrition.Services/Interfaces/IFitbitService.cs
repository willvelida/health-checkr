using HealthCheckr.Nutrition.Common.FitbitResponses;

namespace HealthCheckr.Nutrition.Services.Interfaces
{
    public interface IFitbitService
    {
        Task<FoodResponseObject> GetFoodLogs(string date);
    }
}
