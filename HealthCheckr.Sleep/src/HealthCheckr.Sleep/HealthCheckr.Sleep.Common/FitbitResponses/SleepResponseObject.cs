using System.Diagnostics.CodeAnalysis;

namespace HealthCheckr.Sleep.Common.FitbitResponses
{
    [ExcludeFromCodeCoverage]
    public class SleepResponseObject
    {
        public List<Sleep> sleep { get; set; }
        public Summary summary { get; set; }
    }
}
