using HealthCheckr.Body.Common.FitbitResponses;

namespace HealthCheckr.Body.Services.Interfaces
{
    public interface IFitbitService
    {
        Task<WeightResponseObject> GetWeightResponse(string startDate, string endDate);
        Task<CardioResponseObject> GetV02MaxSummary(string date);
    }
}
