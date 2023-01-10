using HealthCheckr.Sleep.Common.FitbitResponses;

namespace HealthCheckr.Sleep.Services.Interfaces
{
    public interface ISleepService
    {
        Task MapSleepEnvelopeAndSaveToDatabase(SleepResponseObject sleepResponse);
        Task MapSp02RecordAndSaveToDatabase(Sp02ResponseObject sp02Response);
        Task MapBreathingRecordAndSaveToDatabase(BreathingRateResponseObject breathingRateResponseObject);
        Task SendRecordToQueue<T>(T record, string queueName);
    }
}
