using HealthCheckr.Nutrition.Common.FitbitResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCheckr.Nutrition.Services.Interfaces
{
    public interface IFitbitService
    {
        Task<FoodResponseObject> GetFoodLogs(string date);
    }
}
