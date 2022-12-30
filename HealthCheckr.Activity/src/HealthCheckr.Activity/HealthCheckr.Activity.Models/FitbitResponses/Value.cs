namespace HealthCheckr.Activity.Common.FitbitResponses
{
    public class Value
    {
        public List<CustomHeartRateZone> customHeartRateZones { get; set; }
        public List<HeartRateZone> heartRateZones { get; set; }
        public int restingHeartRate { get; set; }
    }
}
