using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    public class SensorController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public SensorController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<SensorItemDto>> GetSensorAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return this._dbContext.GetSensors(userId);
        }

        [HttpGet("{id}")]
        public ActionResult<SensorItemDto> GetSensor(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sensor = this._dbContext.Sensor
            .Where(sensor => (sensor.OwnerId == userId) & (sensor.Id == Guid.Parse(id)))
            .Select(sensor => ToSensorItemDto(sensor))
            .First();

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
            var owner = this._dbContext.Users.Find(userId);
            var sensor = new Sensor() { Owner = owner, Name = sensorItemDto.Name, SensorType = sensorItemDto.SensorType };
            this._dbContext.Add(sensor);
            this._dbContext.SaveChanges();
            return CreatedAtAction(nameof(GetSensor), new { id = sensor.Id }, ToSensorItemDto(sensor));
        }

        private static SensorItemDto ToSensorItemDto(Sensor sensor){
            return new SensorItemDto(){
                Id = sensor.Id.ToString(),
                Name = sensor.Name,
                SensorType = sensor.SensorType
            };
        }
    }

    

}
