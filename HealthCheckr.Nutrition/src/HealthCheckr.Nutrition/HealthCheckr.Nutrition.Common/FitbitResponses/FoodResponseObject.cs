using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCheckr.Nutrition.Common.FitbitResponses
{
    public class FoodResponseObject
    {
        public List<Food> foods { get; set; }
        public Goals goals { get; set; }
        public Summary summary { get; set; }
    }
}
