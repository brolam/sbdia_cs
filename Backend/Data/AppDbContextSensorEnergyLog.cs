
using System;
using Backend.Models;
using Backend.Models.Dtos;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
  public partial class AppDbContext : ApiAuthorizationDbContext<Owner>
  {
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
    public Task<SensorEnergyLogItemDto[]> GetSensorEnergyLogsRecentAsync(Sensor sensor)
    {
      var logs = from log in this.SensorEnergyLogs
      .Where(log => log.SensorId == sensor.Id)
                 select new SensorEnergyLogItemDto()
                 {
                   Id = log.Id,
                   DateTime = sensor.ToDateTimeSensorTimeZone(log.UnixTime),
                   Duration = log.Duration,
                   Watts1 = log.Watts1,
                   Watts2 = log.Watts2,
                   Watts3 = log.Watts3,
                   WattsTotal = log.WattsTotal,
                 };
      return logs.OrderByDescending(log => log.Id).Take(10).ToArrayAsync<SensorEnergyLogItemDto>();
    }

    private void PerformSensorEnergyLogBatch(Sensor sensor, SensorLogBatch sensorLogBatch)
    {
      SensorEnergyLog previousLog = GetSensorEnergyLogLast(sensor);
      try
      {
        foreach (var contentLogItem in sensorLogBatch.Content.Split("|"))
        {
          Func<long, long> getSensorDimTimeId = (unixTime) => this.GetOrCreateSensorDimTime(sensor, unixTime).Id;
          var newEnergyLog = SensorEnergyLog.Parse(sensor, getSensorDimTimeId, contentLogItem);
          if (previousLog != null) newEnergyLog.CalculateDuration(sensor.LogDurationMode, previousLog);
          this.SensorEnergyLogs.Add(newEnergyLog);
          this.SaveOrUpdateSensorEnergyLog(newEnergyLog);
          previousLog = newEnergyLog;
        }
        this.SensorLogBatchs.Remove(sensorLogBatch);
        this.SaveChanges();

      }
      catch (Exception e)
      {
        sensorLogBatch.Attempts++;
        sensorLogBatch.Exception = $"Message: {e.Message}\nSource: {e.Source}";
        this.SaveChanges();
      }
    }

    private void SaveOrUpdateSensorEnergyLog(SensorEnergyLog newEnergyLog)
    {
      try
      {
        this.SaveChanges();
      }
      catch (Exception e)
      {
        this.SensorEnergyLogs.Remove(newEnergyLog);
        var oldEnergyLog = this.SensorEnergyLogs
       .Where(log => (log.SensorId == newEnergyLog.SensorId) && (log.UnixTime == newEnergyLog.UnixTime))
       .FirstOrDefault();
        if (oldEnergyLog != null)
        {
          oldEnergyLog.Update(newEnergyLog);
          this.SaveChanges();
        }
        else
        {
          throw e;
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
    public SensorEnergyLog GetSensorEnergyLogLast(Sensor sensor)
    {
      return this.SensorEnergyLogs
      .Where(log => log.SensorId == sensor.Id)
      .OrderByDescending(log => log.UnixTime)
      .FirstOrDefault();
    }
  }
}