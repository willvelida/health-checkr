using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCheckr.Activity.Common.Envelopes
{
    public class ActivityHeartRateZonesRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Minutes { get; set; }
        public int MaxHR { get; set; }
        public int MinHR { get; set; }
        public double CaloriesOut { get; set; }
        public string Date { get; set; }
    }
}
