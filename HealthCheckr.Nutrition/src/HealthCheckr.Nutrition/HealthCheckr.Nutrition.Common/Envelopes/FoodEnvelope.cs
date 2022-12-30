using HealthCheckr.Nutrition.Common.FitbitResponses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
