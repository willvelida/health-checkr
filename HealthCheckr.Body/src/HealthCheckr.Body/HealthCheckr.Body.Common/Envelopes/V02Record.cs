using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCheckr.Body.Common.Envelopes
{
    public class V02Record
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string V02Max { get; set; }
        public string Date { get; set; }
    }
}
