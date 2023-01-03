using HealthCheckr.Nutrition.Common.FitbitResponses;
using Newtonsoft.Json;

namespace HealthCheckr.Nutrition.Common.Envelopes
{
    public class FoodEnvelope
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public FoodResponseObject Food { get; set; }
        public string Date { get; set; }
        public string DocumentType { get; set; }
    }
}
