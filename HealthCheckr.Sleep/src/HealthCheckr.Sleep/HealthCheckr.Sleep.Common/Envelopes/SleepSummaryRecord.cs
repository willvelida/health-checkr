using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCheckr.Sleep.Common.Envelopes
{
    public class SleepSummaryRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TotalMinutesAsleep { get; set; }
        public int TotalSleepRecords { get; set; }
        public int TotalTimeInBed { get; set; }
        public int DeepSleep { get; set; }
        public int LightSleep { get; set; }
        public int REMSleep { get; set; }
        public int AwakeMinutes { get; set; }
        public string Date { get; set; }
    }
}
