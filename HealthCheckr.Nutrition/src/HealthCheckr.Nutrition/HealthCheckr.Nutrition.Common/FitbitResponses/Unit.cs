using System.Diagnostics.CodeAnalysis;

namespace HealthCheckr.Nutrition.Common.FitbitResponses
{
    [ExcludeFromCodeCoverage]
    public class Unit
    {
        public int id { get; set; }
        public string name { get; set; }
        public string plural { get; set; }
    }
}
