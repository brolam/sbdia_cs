using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class SensorLogBatch
    {
        public long Id { get; set; }
        [Required]
        public string SensorId { get; set; }
        [Required]
        public Guid SecretApiToken { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public int Attempts { get; set; } = 1;
        public string Exception { get; set; }
    }
}
