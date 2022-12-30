using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
