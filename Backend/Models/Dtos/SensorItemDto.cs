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
    public string SensorTypeName { get; set; }
    public float LogDurationMode { get; set; }
    public DateTime LogLastRecorded { get; set; }
    public string SecretApiToken { get; set; }
    public string TimeZone { get; set; }
  }
}
