using HealthCheckr.Nutrition.Common.Envelopes;

namespace HealthCheckr.Nutrition.Repository.Interfaces
{
    public interface ICosmosDbRepository
    {
        Task CreateFoodDocument(FoodEnvelope foodEnvelope);
    }
}
