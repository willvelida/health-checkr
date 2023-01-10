using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCheckr.Sleep.Common.Envelopes
{
    public class BreathingRateRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Date { get; set; }
        public double BreathingRate { get; set; }
    }
}
