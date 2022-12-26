using System.Diagnostics.CodeAnalysis;

namespace HealthCheckr.Activity.Common.FitbitResponses
{
    [ExcludeFromCodeCoverage]
    public class Distance
    {
        public string activity { get; set; }
        public double distance { get; set; }
    }
}
