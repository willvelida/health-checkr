using HealthCheckr.Nutrition.Common.Envelopes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCheckr.Nutrition.Repository.Interfaces
{
    public interface ICosmosDbRepository
    {
        Task CreateFoodDocument(FoodEnvelope foodEnvelope);
    }
}
