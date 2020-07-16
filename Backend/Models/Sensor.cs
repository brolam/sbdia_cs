using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public enum SensorTypes
    {
        EnergyLog = 1
    }
    public class Sensor
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required] 
        public string OwnerId { get; set; }
        [Required, ForeignKey("OwnerId")]
        public virtual ApplicationUser Owner { get; set; }
        public SensorTypes SensorType { get; set; }
        [Required, MaxLength(50)] 
        public string Name { get; set; }
        [Required, MaxLength(200)] 
        public string TimeZone { get; set; } = "America/Recife";
        [Required] 
        public float DefaultToConvert { get; set; } = 26.378f;
        [Required] 
        public float LogDurationMode { get; set; } = 14.00f;
        [Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SecretApiToken { get; set; } = Guid.NewGuid();
        public List<SensorCost> Costs { get; set; }
    }
}
