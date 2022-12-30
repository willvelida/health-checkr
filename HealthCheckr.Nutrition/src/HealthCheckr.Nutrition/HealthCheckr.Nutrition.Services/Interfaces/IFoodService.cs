using HealthCheckr.Nutrition.Common.FitbitResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCheckr.Nutrition.Services.Interfaces
{
    public interface IFoodService
    {
        Task SendFoodRecordToQueue(FoodResponseObject foodResponseObject);
        Task MapFoodEnvelopeAndSaveToDatabase(FoodResponseObject foodResponseObject);
    }
}
