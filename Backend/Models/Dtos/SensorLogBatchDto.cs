using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Dtos
{
    public class SensorLogBatchDto
    {
        [Required]
        public string SensorId { get; set; }
        [Required]
        public string SecretApiToken { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
