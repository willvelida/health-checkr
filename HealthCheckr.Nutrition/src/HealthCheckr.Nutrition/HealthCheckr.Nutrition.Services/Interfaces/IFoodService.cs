using HealthCheckr.Nutrition.Common.FitbitResponses;

namespace HealthCheckr.Nutrition.Services.Interfaces
{
    public interface IFoodService
    {
        Task SendFoodRecordToQueue(FoodResponseObject foodResponseObject);
        Task MapFoodEnvelopeAndSaveToDatabase(FoodResponseObject foodResponseObject);
    }
}
