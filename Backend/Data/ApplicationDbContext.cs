using System.Linq;
using Backend.Models;
using Backend.Models.Dtos;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Backend.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        private DbSet<Sensor> Sensor { get; set; }

        public SensorItemDto[] GetSensors(string onwerId)
        {
            var sensors = from sensor in this.Sensor
            .Where(sensor => sensor.OwnerId == onwerId)
            select new SensorItemDto(){
                Id = sensor.Id.ToString(),
                Name = sensor.Name,
                SensorType = sensor.SensorType
            }; 
            return sensors.ToArray();
        }
        
        public Sensor CreateSensor(string userId, SensorItemDto sensorItemDto){
            var owner = this.Users.Find(userId);
            var sensor = new Sensor() { Owner = owner, Name = sensorItemDto.Name, SensorType = sensorItemDto.SensorType };
            this.Add(sensor);
            this.SaveChanges();
            return sensor;
        }

        public SensorItemDto GetSensor(string id)
        {
            var sensors = from sensor in this.Sensor
            .Where(sensor => sensor.Id == System.Guid.Parse(id))
            select new SensorItemDto(){
                Id = sensor.Id.ToString(),
                Name = sensor.Name,
                SensorType = sensor.SensorType
            }; 
            return sensors.First();
        }
    }
}
