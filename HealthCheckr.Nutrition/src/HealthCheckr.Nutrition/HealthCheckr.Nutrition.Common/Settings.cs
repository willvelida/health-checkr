using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCheckr.Nutrition.Common
{
    public class Settings
    {
        public string NutritionQueueName { get; set; }
        public string DatabaseName { get; set; }
        public string ContainerName { get; set; }
    }
}
