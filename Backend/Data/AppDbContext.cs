using System;
using System.Linq;
using Backend.Models;
using Backend.Models.Dtos;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Backend.Data
{
    public class AppDbContext : ApiAuthorizationDbContext<Owner>
    {
        public AppDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }
        private DbSet<Sensor> Sensors { get; set; }
        private DbSet<SensorCost> SensorCosts { get; set; }
        private DbSet<SensorDimTime> SensorDimTimes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Sensor>()
            .HasOne<Owner>()
            .WithMany()
            .HasForeignKey(sensor => sensor.OwnerId)
            .IsRequired();

            modelBuilder.Entity<SensorCost>()
            .HasOne<Sensor>()
            .WithMany()
            .HasForeignKey(sensorCost => sensorCost.SensorId)
            .IsRequired();

            modelBuilder.Entity<SensorDimTime>()
            .HasOne<Sensor>()
            .WithMany()
            .HasForeignKey(sensorDimTime => sensorDimTime.SensorId)
            .IsRequired();

            modelBuilder.Entity<SensorDimTime>()
           .HasOne<SensorCost>()
           .WithMany()
           .HasForeignKey(sensorDimTime => sensorDimTime.SensorCostId)
           .OnDelete(DeleteBehavior.Restrict)
           .IsRequired();
        }
        public SensorItemDto[] GetSensors(string onwerId)
        {
            var sensors = from sensor in this.Sensors
            .Where(sensor => sensor.OwnerId == onwerId)
                          select new SensorItemDto()
                          {
                              Id = sensor.Id.ToString(),
                              Name = sensor.Name,
                              SensorType = sensor.SensorType
                          };
            return sensors.ToArray();
        }

        public Sensor CreateSensor(string userId, SensorItemDto sensorItemDto)
        {
            var owner = this.Users.Find(userId);
            var sensor = new Sensor() { OwnerId = owner.Id, Name = sensorItemDto.Name, SensorType = sensorItemDto.SensorType };
            this.Add(sensor);
            this.SaveChanges();
            return sensor;
        }
        public SensorItemDto GetSensor(string id)
        {
            var sensors = from sensor in this.Sensors
            .Where(sensor => sensor.Id == id)
                          select new SensorItemDto()
                          {
                              Id = sensor.Id.ToString(),
                              Name = sensor.Name,
                              SensorType = sensor.SensorType
                          };
            return sensors.First();
        }
        public SensorCost GetSensorCost(string sensorId, long id)
        {
            return this.SensorCosts.First(sensorCost => sensorCost.SensorId == sensorId && sensorCost.Id == id);
         
        }

        public SensorCost GetLastOrCreateSensorCost(string sensorId)
        {
            var lastSensorCost = this.SensorCosts
            .OrderByDescending(sensorCost => sensorCost.Id)
            .FirstOrDefault(sensorCost => sensorCost.SensorId == sensorId);
            if (lastSensorCost == null)
            {
                lastSensorCost = new SensorCost() { SensorId = sensorId, Title = "$", Value = 0.0F };
                this.SensorCosts.Add(lastSensorCost);
                this.SaveChanges();
            }
            return lastSensorCost;
        }
        public SensorDimTime GetSensorDimTime(string SensorId, long id)
        {
            return this.SensorDimTimes
            .First(sensorCost => sensorCost.SensorId == SensorId && sensorCost.Id == id);
           
        }
        public SensorDimTime GetOrCreateSensorDimTime(long unixTimeSeconds, Sensor sensor)
        {
            var sensorCost = GetLastOrCreateSensorCost(sensor.Id);
            var sensorDimTime = new SensorDimTime(unixTimeSeconds, sensor, sensorCost.Id);
            this.SensorDimTimes.Add(sensorDimTime);
            this.SaveChanges();
            return sensorDimTime;
        }
    }
}
