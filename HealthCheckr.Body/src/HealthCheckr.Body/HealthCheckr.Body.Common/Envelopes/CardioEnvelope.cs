using HealthCheckr.Body.Common.FitbitResponses;
using Newtonsoft.Json;

namespace HealthCheckr.Body.Common.Envelopes
{
    public class CardioEnvelope
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public CardioResponseObject Cardio { get; set; }
        public string Date { get; set; }
        public string DocumentType { get; set; }
    }
}
