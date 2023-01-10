using HealthCheckr.Sleep.Common.Envelopes;
using Microsoft.EntityFrameworkCore;

namespace HealthCheckr.Sleep.Repository
{
    public class SleepContext : DbContext
    {
        public SleepContext(DbContextOptions<SleepContext> options) : base(options)
        {

        }

        public DbSet<Sp02Record> Sp02 { get; set; }
        public DbSet<BreathingRateRecord> BreathingRate { get; set; }
    }
}
