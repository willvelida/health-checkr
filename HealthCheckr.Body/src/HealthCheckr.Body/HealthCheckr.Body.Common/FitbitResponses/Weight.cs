using System.Diagnostics.CodeAnalysis;

namespace HealthCheckr.Body.Common.FitbitResponses
{
    [ExcludeFromCodeCoverage]
    public class Weight
    {
        public double bmi { get; set; }
        public string date { get; set; }
        public double fat { get; set; }
        public object logId { get; set; }
        public string source { get; set; }
        public string time { get; set; }
        public double weight { get; set; }
    }
}
