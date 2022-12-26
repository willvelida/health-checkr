using Newtonsoft.Json;

namespace HealthCheckr.Activity.Common.Envelopes
{
    public class ActivityEnvelope
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public Activity Activity { get; set; }
        public string Date { get; set; }
        public string DocumentType { get; set; }
    }
}
