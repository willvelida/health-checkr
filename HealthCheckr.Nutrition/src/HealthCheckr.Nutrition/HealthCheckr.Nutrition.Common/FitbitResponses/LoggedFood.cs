﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCheckr.Nutrition.Common.FitbitResponses
{
    [ExcludeFromCodeCoverage]
    public class LoggedFood
    {
        public string accessLevel { get; set; }
        public int amount { get; set; }
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
