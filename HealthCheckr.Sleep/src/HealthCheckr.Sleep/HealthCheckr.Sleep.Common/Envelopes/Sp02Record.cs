using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCheckr.Sleep.Common.Envelopes
{
    public class Sp02Record
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Date { get; set; }
        public double AvgValue { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
    }
}
