﻿using System;
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
  public partial class AppDbContext : ApiAuthorizationDbContext<Owner>
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

      modelBuilder.Entity<SensorEnergyLog>()
      .HasIndex(log => new { log.SensorId, log.UnixTime })
      .IsUnique(true);
    }

    #region Sensor
    public async Task<Sensor> GetSensorAsync(string sensorId)
    {
      return await this.Sensors.FindAsync(sensorId);
    }
    public async Task<SensorItemDto[]> GetSensors(string onwerId)
    {
      var sensors = from sensor in this.Sensors
      .Where(sensor => sensor.OwnerId == onwerId)
                    select new SensorItemDto()
                    {
                      Id = sensor.Id.ToString(),
                      Name = sensor.Name,
                      SensorType = sensor.SensorType,
                      SensorTypeName = sensor.SensorType.ToString(),
                      LogDurationMode = sensor.LogDurationMode,
                      LogLastRecorded = new DateTime(2020, 10, 11, 12, 55, 59),
                      SecretApiToken = sensor.SecretApiToken
                    };
      return await sensors.ToArrayAsync();
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
      var sensor = new Sensor()
      {
        OwnerId = owner.Id,
        Name = sensorItemDto.Name,
        SensorType = sensorItemDto.SensorType,
        TimeZone = sensorItemDto.TimeZone
      };
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
    public SensorDimTime GetSensorDimTime(string sensorId, DateTime dateTime)
    {
      return this.SensorDimTimes
      .FirstOrDefault
      (
        sensorDimTime =>
        (
          sensorDimTime.SensorId == sensorId &&
          sensorDimTime.Year == dateTime.Year &&
          sensorDimTime.Month == dateTime.Month &&
          sensorDimTime.Day == dateTime.Day &&
          sensorDimTime.Hour == dateTime.Hour
        )
      );
    }
    public SensorDimTime GetOrCreateSensorDimTime(Sensor sensor, long unixTimeSeconds)
    {
      DateTime dateTime = sensor.ToDateTimeSensorTimeZone(unixTimeSeconds);
      var sensorDimTime = GetSensorDimTime(sensor.Id, dateTime);
      if (sensorDimTime == null)
      {
        var sensorCost = GetLastOrCreateSensorCost(sensor.Id);
        sensorDimTime = new SensorDimTime(sensor, dateTime, sensorCost.Id);
        this.SensorDimTimes.Add(sensorDimTime);
        this.SaveChanges();
      }
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
      .Where(sensorLogBatch => sensorLogBatch.SensorId.Equals(sensorId) && sensorLogBatch.Attempts <= 3)
      .OrderBy(sensorLogBatch => sensorLogBatch.Id);
      return sensorLogBatchPending.ToArray();
    }
    #endregion SensorLogBatch

    #region Business Processes 
    public void PerformContentSensorLogBatch(Sensor sensor)
    {
      if (sensor.SensorType == SensorTypes.EnergyLog)
      {
        var sensorLogBatchPending = this.GetSensorLogBatchPending(sensor.Id);
        foreach (var sensorLogBatch in sensorLogBatchPending)
        {
          if (sensor.SensorType == SensorTypes.EnergyLog)
          {
            PerformSensorEnergyLogBatch(sensor, sensorLogBatch);
          }
        }
        this.UpdateSensorEnergyLogDurationMode(sensor);
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