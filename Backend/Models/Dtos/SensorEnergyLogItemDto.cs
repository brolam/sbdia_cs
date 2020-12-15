using System;

namespace Backend.Models.Dtos
{
  public class SensorEnergyLogItemDto
  {
    public long Id { get; set; }
    public long UnixTime { get; set; }
    public float Duration { get; set; }
    public float Watts1 { get; set; }
    public float Watts2 { get; set; }
    public float Watts3 { get; set; }
    public float WattsTotal { get; set; }
  }
}
