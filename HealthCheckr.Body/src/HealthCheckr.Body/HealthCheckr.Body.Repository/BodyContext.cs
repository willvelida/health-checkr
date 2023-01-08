using HealthCheckr.Body.Common.Envelopes;
using Microsoft.EntityFrameworkCore;

namespace HealthCheckr.Body.Repository
{
    public class BodyContext : DbContext
    {
        public BodyContext(DbContextOptions<BodyContext> options) : base(options)
        {

        }

        public DbSet<V02Record> V02 { get; set; }
        public DbSet<WeightRecord> Weight { get; set; }
    }
}
