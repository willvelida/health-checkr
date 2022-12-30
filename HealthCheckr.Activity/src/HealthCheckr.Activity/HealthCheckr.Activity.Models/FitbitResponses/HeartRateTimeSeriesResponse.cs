using Newtonsoft.Json;

namespace HealthCheckr.Activity.Common.FitbitResponses
{
    public class HeartRateTimeSeriesResponse
    {
        [JsonProperty("activities-heart")]
        public List<ActivitiesHeart> activitiesheart { get; set; }
    }
}
