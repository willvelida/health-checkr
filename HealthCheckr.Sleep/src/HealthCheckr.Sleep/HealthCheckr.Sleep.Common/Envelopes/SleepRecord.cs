using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCheckr.Sleep.Common.Envelopes
{
    public class SleepRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Date { get; set; }
        public int Efficiency { get; set; }
        public int Duration { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int MinutesAfterWakeup { get; set; }
        public int MinutesAsleep { get; set; }
        public int MinutesAwake { get; set; }
        public int MinutesToFallAsleep { get; set; }
        public int RestlessCount { get; set; }
        public int RestlessDuration { get; set; }
        public int TimeInBed { get; set; }
        public int SleepSummaryId { get; set; }
    }
}
