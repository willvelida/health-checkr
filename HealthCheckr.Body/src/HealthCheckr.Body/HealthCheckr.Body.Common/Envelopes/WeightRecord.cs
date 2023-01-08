using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCheckr.Body.Common.Envelopes
{
    public class WeightRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public double BmiValue { get; set; }
        public string Date { get; set; }
        public double FatPercentage { get; set; }
        public string Source { get; set; }
        public string Time { get; set; }
        public double WeightInKG { get; set; }
    }
}
