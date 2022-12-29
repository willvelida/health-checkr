using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCheckr.Body.Common.FitbitResponses
{
    [ExcludeFromCodeCoverage]
    public class WeightResponseObject
    {
        public List<Weight> weight { get; set; }
    }
}
