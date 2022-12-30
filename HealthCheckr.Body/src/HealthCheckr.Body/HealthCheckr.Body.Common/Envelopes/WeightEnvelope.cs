﻿using HealthCheckr.Body.Common.FitbitResponses;
using Newtonsoft.Json;

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
