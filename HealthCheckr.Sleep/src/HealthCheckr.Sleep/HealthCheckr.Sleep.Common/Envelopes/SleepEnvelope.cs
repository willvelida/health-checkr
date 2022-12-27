using HealthCheckr.Sleep.Common.FitbitResponses;

namespace HealthCheckr.Sleep.Common.Envelopes
{
    public class SleepEnvelope
    {
        public string Id { get; set; }
        public SleepResponseObject Sleep { get; set; }
        public string Date { get; set; }
        public string DocumentType { get; set; }
    }
}
