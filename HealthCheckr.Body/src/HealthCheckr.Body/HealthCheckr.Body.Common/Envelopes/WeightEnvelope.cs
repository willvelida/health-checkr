using HealthCheckr.Body.Common.FitbitResponses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCheckr.Body.Common.Envelopes
{
    public class WeightEnvelope
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public Weight Weight { get; set; }
        public string Date { get; set; }
        public string DocumentType { get; set; }
    }
}
