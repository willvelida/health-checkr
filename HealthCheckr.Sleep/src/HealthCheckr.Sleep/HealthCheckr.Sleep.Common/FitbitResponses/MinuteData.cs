using System.Diagnostics.CodeAnalysis;

namespace HealthCheckr.Sleep.Common.FitbitResponses
{
    [ExcludeFromCodeCoverage]
    public class MinuteData
    {
        public string dateTime { get; set; }
        public string value { get; set; }
    }
}
