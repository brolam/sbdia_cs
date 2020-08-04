using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class SensorEnergyLog
    {
        public long Id { get; set; }
        [Required]
        public string SensorId { get; set; }
        [Required]
        public long UnixTime { get; set; }
        [Required, Range(0.00, float.MaxValue)]
        public float Duration { get; set; }
        [Required, Range(0.00, float.MaxValue)]
        public float Watts1 { get; set; }
        [Required, Range(0.00, float.MaxValue)]
        public float Watts2 { get; set; }
        [Required, Range(0.00, float.MaxValue)]
        public float Watts3 { get; set; }
        [Required, Range(0.00, float.MaxValue)]
        public float WattsTotal { get; set; }
        [Required]
        public float ConvertToUnits { get; set; }
    }
}
