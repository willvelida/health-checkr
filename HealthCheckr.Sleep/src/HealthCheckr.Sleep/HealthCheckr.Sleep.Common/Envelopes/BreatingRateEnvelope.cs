using HealthCheckr.Sleep.Common.FitbitResponses;
using Newtonsoft.Json;

namespace HealthCheckr.Sleep.Common.Envelopes
{
    public class BreatingRateEnvelope
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public BreathingRateResponseObject BreathingRate { get; set; }
        public string Date { get; set; }
        public string DocumentType { get; set; }
    }
}
