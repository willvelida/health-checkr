using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCheckr.Nutrition.Common.FitbitResponses
{
    [ExcludeFromCodeCoverage]
    public class Summary
    {
        public int calories { get; set; }
        public double carbs { get; set; }
        public double fat { get; set; }
        public double fiber { get; set; }
        public double protein { get; set; }
        public double sodium { get; set; }
        public int water { get; set; }
    }
}
