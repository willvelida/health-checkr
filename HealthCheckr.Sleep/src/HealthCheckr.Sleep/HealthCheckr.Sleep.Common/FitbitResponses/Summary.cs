using System.Diagnostics.CodeAnalysis;

namespace HealthCheckr.Sleep.Common.FitbitResponses
{
    [ExcludeFromCodeCoverage]
    public class Summary
    {
        public Stages stages { get; set; }
        public int totalMinutesAsleep { get; set; }
        public int totalSleepRecords { get; set; }
        public int totalTimeInBed { get; set; }
    }
}
