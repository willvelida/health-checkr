using HealthCheckr.Activity.Common.FitbitResponses;
using Newtonsoft.Json;

namespace HealthCheckr.Activity.Common.Envelopes
{
    public class ActivityEnvelope
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public ActivityResponse Activity { get; set; }
        public string Date { get; set; }
        public string DocumentType { get; set; }
    }
}
