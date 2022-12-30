using HealthCheckr.Sleep.Common.FitbitResponses;

namespace HealthCheckr.Sleep.Services.Interfaces
{
    public interface ISleepService
    {
        Task MapAndSendSleepRecordToQueue(SleepResponseObject sleepResponse);
        Task MapSleepEnvelopeAndSaveToDatabase(SleepResponseObject sleepResponse);
        Task SendSp02RecordToQueue(Sp02ResponseObject sp02Response);
        Task MapSp02EnvelopeAndSaveToDatabase(Sp02ResponseObject sp02Response);
        Task SendBreathingResponseToQueue(BreathingRateResponseObject breathingRateResponseObject);
        Task MapBreathingEnvelopeAndSaveToDatabase(BreathingRateResponseObject breathingRateResponseObject);
    }
}
