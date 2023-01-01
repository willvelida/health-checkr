using HealthCheckr.Body.Common.FitbitResponses;

namespace HealthCheckr.Body.Services.Interfaces
{
    public interface IBodyService
    {
        Task MapWeightEnvelopeAndSaveToDatabase(Weight weight);
        Task MapCardioEnvelopeAndSaveToDatabase(CardioResponseObject cardioResponseObject);
        Task SendRecordToQueue<T>(T record, string queueName);
    }
}
