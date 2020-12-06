
namespace Backend.Models.Dtos
{
  public class SensorXyDto
  {
    public SensorXyDto(float x, float y)
    {
      this.X = x;
      this.Y = y;
    }

    public float X { get; set; }
    public float Y { get; set; }
  }
}