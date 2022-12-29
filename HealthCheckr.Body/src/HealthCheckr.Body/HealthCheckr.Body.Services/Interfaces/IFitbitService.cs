using HealthCheckr.Body.Common.FitbitResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCheckr.Body.Services.Interfaces
{
    public interface IFitbitService
    {
        Task<WeightResponseObject> GetWeightResponse(string startDate, string endDate);
    }
}
