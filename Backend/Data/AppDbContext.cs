using System;
using System.Linq;
using System.Threading.Tasks;
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
        private DbSet<SensorLogBatch> SensorLogBatchs { get; set; }
        private DbSet<SensorEnergyLog> SensorEnergyLogs { get; set; }

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

        #region Sensor
        internal async Task<Sensor> GetSensor(string sensorId)
        {
            return await this.Sensors.FindAsync(sensorId);
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

        public SensorItemDto GetSensorItemDto(string id)
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
        public SensorDto GetSensorDto(string id)
        {
            var sensors = from sensor in this.Sensors
            .Where(sensor => sensor.Id == id)
                          select new SensorDto()
                          {
                              Id = sensor.Id,
                              LogDurationMode = sensor.LogDurationMode,
                              SecretApiToken = sensor.SecretApiToken.ToString()
                          };
            return sensors.First();
        }
        public Sensor CreateSensor(string userId, SensorItemDto sensorItemDto)
        {
            var owner = this.Users.Find(userId);
            var sensor = new Sensor() { OwnerId = owner.Id, Name = sensorItemDto.Name, SensorType = sensorItemDto.SensorType };
            this.Add(sensor);
            this.SaveChanges();
            return sensor;
        }

        #endregion Sensor

        #region SensorCost
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
        #endregion SensorCost

        #region SensorDimTime
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
        #endregion SensorDimTime

        #region SensorLogBatch
        public async Task<int> CreateSensorLogBatch(Sensor sensor, string content)
        {
            var sensorLogBatch = new SensorLogBatch()
            {
                SensorId = sensor.Id,
                SecretApiToken = sensor.SecretApiToken,
                Content = content,
                Attempts = 0
            };
            this.SensorLogBatchs.Add(sensorLogBatch);
            return await this.SaveChangesAsync();
        }
        public SensorLogBatch[] GetSensorLogBatchPending(string sensorId)
        {
            var sensorLogBatchPending = this.SensorLogBatchs
            .Where(sensorLogBatch => sensorLogBatch.SensorId.Equals(sensorId) && sensorLogBatch.Attempts < 3)
            .OrderBy(sensorLogBatch => sensorLogBatch.Id);
            return sensorLogBatchPending.ToArray();
        }
        #endregion SensorLogBatch

        #region SensorEnergyLog
        public SensorEnergyLog CreateSensorEnergyLog(string sensorId, long sensorDimTimeId, long unixTime, float duration, float watts1, float watts2, float watts3, float convertToUnits)
        {
            var createSensorEnergyLog = new SensorEnergyLog()
            {
                SensorId = sensorId,
                SensorDimTimeId = sensorDimTimeId,
                UnixTime = unixTime,
                Duration = duration,
                Watts1 = watts1,
                Watts2 = watts2,
                Watts3 = watts3,
                WattsTotal = watts1 + watts2 + watts3,
                ConvertToUnits = convertToUnits
            };
            this.SensorEnergyLogs.Add(createSensorEnergyLog);
            this.SaveChanges();
            return createSensorEnergyLog;
        }
        public SensorEnergyLogItemDto[] GetSensorEnergyLogsRecent(Sensor sensor)
        {
            var logs = from log in this.SensorEnergyLogs
            .Where(log => log.SensorId == sensor.Id)
                       select new SensorEnergyLogItemDto()
                       {
                           Id = log.Id,
                           Duration = log.Duration,
                           Watts1 = log.Watts1,
                           Watts2 = log.Watts2,
                           Watts3 = log.Watts3,
                           WattsTotal = log.WattsTotal,
                       };
            return logs.OrderByDescending(log => log.Id).Take(10).ToArray<SensorEnergyLogItemDto>();
        }
        #endregion SensorEnergyLog

        #region Business Processes 
        public void PerformContentSensorLogBatch(Sensor sensor)
        {
            if (sensor.SensorType == SensorTypes.EnergyLog)
            {
                var sensorLogBatchPending = this.GetSensorLogBatchPending(sensor.Id);
                foreach (var sensorLogBatch in sensorLogBatchPending)
                {
                    SensorEnergyLog previousLog = null;
                    foreach (var contentLogItem in sensorLogBatch.Content.Split("|"))
                    {
                        var energyLog = SensorEnergyLog.Parse(sensor, ((unixTime) => this.GetOrCreateSensorDimTime(unixTime, sensor).Id), contentLogItem);
                        if (previousLog != null) energyLog.CalculateDuration(previousLog);
                        this.SensorEnergyLogs.Add(energyLog);
                        previousLog = energyLog;
                    }
                    this.SensorLogBatchs.Remove(sensorLogBatch);
                    this.SaveChanges();
                    this.UpdateSensorEnergyLogDurationMode(sensor);
                }
            }
        }
        private void UpdateSensorEnergyLogDurationMode(Sensor sensor)
        {
            var mode = this.SensorEnergyLogs
            .Where(log => log.SensorId == sensor.Id)
            .Take(100)
            .GroupBy(log => log.Duration)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key).FirstOrDefault();
            if (mode > 0)
            {
                sensor.LogDurationMode = mode;
                this.SaveChanges();
            }
        }
        public async Task<Sensor[]> GetSensorWithLogBatchPending()
        {
            return await this.Sensors
            .Where
            (
                sensor => this.SensorLogBatchs.Any
                (
                    logBatch => ((logBatch.SensorId == sensor.Id) && (logBatch.Attempts < 3))
                )
            ).ToArrayAsync();
        }
    }
}
#endregion Business Processes