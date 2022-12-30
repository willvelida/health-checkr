using HealthCheckr.Body.Common.FitbitResponses;

namespace HealthCheckr.Body.Services.Interfaces
{
    public interface IBodyService
    {
        Task MapAndSendWeightRecordToQueue(Weight weight);
        Task MapWeightEnvelopeAndSaveToDatabase(Weight weight);
        Task SendCardioResponseObjectToQueue(CardioResponseObject cardioResponseObject);
        Task MapCardioEnvelopeAndSaveToDatabase(CardioResponseObject cardioResponseObject);
    }
}
