using System.Diagnostics.CodeAnalysis;

namespace HealthCheckr.Sleep.Common.FitbitResponses
{
    [ExcludeFromCodeCoverage]
    public class Stages
    {
        public int deep { get; set; }
        public int light { get; set; }
        public int rem { get; set; }
        public int wake { get; set; }
    }
}
