using HealthCheckr.Sleep.Common.FitbitResponses;
using Newtonsoft.Json;

namespace HealthCheckr.Sleep.Common.Envelopes
{
    public class Sp02Envelope
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public Sp02ResponseObject Sp02 { get; set; }
        public string Date { get; set; }
        public string DocumentType { get; set; }
    }
}
