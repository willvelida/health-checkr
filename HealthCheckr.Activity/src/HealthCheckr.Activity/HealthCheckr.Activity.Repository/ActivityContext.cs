using HealthCheckr.Activity.Common.Envelopes;
using Microsoft.EntityFrameworkCore;

namespace HealthCheckr.Activity.Repository
{
    public class ActivityContext : DbContext
    {
        public ActivityContext(DbContextOptions<ActivityContext> options) : base(options)
        {

        }

        public DbSet<ActivityHeartRateZonesRecord> ActivityHeartRateZones { get; set; }
        public DbSet<ActivityDistancesRecord> ActivityDistances { get; set; }
        public DbSet<ActivitySummaryRecord> ActivitySummary { get; set; }
        public DbSet<ActivityRecord> Activity { get; set; }
    }
}
