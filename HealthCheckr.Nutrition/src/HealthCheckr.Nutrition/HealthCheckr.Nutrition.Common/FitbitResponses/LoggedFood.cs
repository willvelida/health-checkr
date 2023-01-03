using System.Diagnostics.CodeAnalysis;

namespace HealthCheckr.Nutrition.Common.FitbitResponses
{
    [ExcludeFromCodeCoverage]
    public class LoggedFood
    {
        public string accessLevel { get; set; }
        public double amount { get; set; }
        public string brand { get; set; }
        public int calories { get; set; }
        public int foodId { get; set; }
        public string locale { get; set; }
        public int mealTypeId { get; set; }
        public string name { get; set; }
        public Unit unit { get; set; }
        public List<int> units { get; set; }
        public string creatorEncodedId { get; set; }
    }
}
