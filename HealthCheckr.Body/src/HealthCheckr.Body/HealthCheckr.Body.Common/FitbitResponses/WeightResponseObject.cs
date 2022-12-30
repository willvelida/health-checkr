using System.Diagnostics.CodeAnalysis;

namespace HealthCheckr.Body.Common.FitbitResponses
{
    [ExcludeFromCodeCoverage]
    public class WeightResponseObject
    {
        public List<Weight> weight { get; set; }
    }
}
