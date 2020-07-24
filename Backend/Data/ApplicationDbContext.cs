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

        public DbSet<Sensor> Sensor { get; set; }

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
    }
}
