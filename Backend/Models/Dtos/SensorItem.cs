using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.Dtos
{

    public class SensorItemDto
    {
        public string Id { get; set; }
        public SensorTypes SensorType { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; }
    }
}
