using System.Diagnostics.CodeAnalysis;

namespace HealthCheckr.Nutrition.Common.FitbitResponses
{
    [ExcludeFromCodeCoverage]
    public class Food
    {
        public bool isFavorite { get; set; }
        public string logDate { get; set; }
        public object logId { get; set; }
        public LoggedFood loggedFood { get; set; }
        public NutritionalValues nutritionalValues { get; set; }
    }
}
