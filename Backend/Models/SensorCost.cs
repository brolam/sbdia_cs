using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class SensorCost
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string SensorId { get; set; }
        [Required, MaxLength(15)]
        public string Title { get; set; }
        [Required]
        public float Value { get; set;}
    }
}
