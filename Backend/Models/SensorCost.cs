using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class SensorCost
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public Sensor Sensor { get; set; }
        [Required, MaxLength(15)]
        public string Title { get; set; }
        [Required]
        public float Value { get; set;}
    }
}
