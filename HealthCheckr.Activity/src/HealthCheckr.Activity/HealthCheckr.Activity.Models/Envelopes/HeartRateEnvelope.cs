using HealthCheckr.Activity.Common.FitbitResponses;
using Newtonsoft.Json;

namespace HealthCheckr.Activity.Common.Envelopes
{
    public class HeartRateEnvelope
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public HeartRateTimeSeriesResponse HeartRate { get; set; }
        public string Date { get; set; }
        public string DocumentType { get; set; }
    }
}
