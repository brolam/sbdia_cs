
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Models;
using Backend.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public partial class SensorController : ControllerBase
  {
    private readonly AppDbContext _dbContext;

    public SensorController(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SensorItemDto>>> GetSensorAll()
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      return await this._dbContext.GetSensors(userId);
    }

    [HttpGet("{id}")]
    public ActionResult<SensorItemDto> GetSensor(string id)
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      var sensor = this._dbContext.GetSensorItemDto(id);
      if (sensor == null)
      {
        return NotFound();
      }

      return sensor;
    }

    [HttpPost]
    public ActionResult<SensorItemDto> CreateSensor(SensorItemDto sensorItemDto)
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      var sensor = this._dbContext.CreateSensor(userId, sensorItemDto);
      return CreatedAtAction(nameof(GetSensor), new { id = sensor.Id }, ToSensorItemDto(sensor));
    }

    private static SensorItemDto ToSensorItemDto(Sensor sensor)
    {
      return new SensorItemDto()
      {
        Id = sensor.Id.ToString(),
        Name = sensor.Name,
        SensorType = sensor.SensorType,
        SecretApiToken = sensor.SecretApiToken
      };
    }

    [HttpPost("logBatch")]
    [AllowAnonymous]
    public async Task<IActionResult> PostSensorLogBatch(SensorLogBatchDto sensorLogBatch)
    {
      var sensor = await this._dbContext.GetSensorAsync(sensorLogBatch.SensorId);
      if (sensor == null) return NotFound();
      var secretApiTokenValid = sensor.SecretApiToken.ToString();
      if (sensorLogBatch.SecretApiToken.Equals(secretApiTokenValid))
      {
        await this._dbContext.CreateSensorLogBatch(sensor, sensorLogBatch.Content);
        return new ObjectResult(null) { StatusCode = 201 };
      }
      return NotFound();
    }
  }



}
