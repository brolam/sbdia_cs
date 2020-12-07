using System.Threading.Tasks;
using Backend.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
  public partial class SensorController : ControllerBase
  {

    [HttpGet("{id}/xy/{year:int}/{month:int}/{day:int}")]
    public async Task<ActionResult<SensorXyDto[]>> GetSensorXyAsync(string id, int year, int month, int day)
    {
      var sensor = await this._dbContext.GetSensorAsync(id);
      var xy = await this._dbContext.GetSensorXyAsync(sensor, year, month, day);
      return xy;
    }
  }
}