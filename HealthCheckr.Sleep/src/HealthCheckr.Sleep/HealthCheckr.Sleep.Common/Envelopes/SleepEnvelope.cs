using HealthCheckr.Sleep.Common.FitbitResponses;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace HealthCheckr.Sleep.Common.Envelopes
{
    public class SleepEnvelope
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public SleepResponseObject Sleep { get; set; }
        public string Date { get; set; }
        public string DocumentType { get; set; }
    }
}
