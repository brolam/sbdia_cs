
using System;
using Backend.Models;
using Backend.Models.Dtos;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace Backend.Data
{
  public partial class AppDbContext : ApiAuthorizationDbContext<Owner>
  {
    public async Task<SensorXyDto[]> GetSensorXyTotalKwhAsync(Sensor sensor, int year, int month, int day)
    {
      var hours = this.SensorDimTimes
      .Where(sensorDimTimes => sensorDimTimes.SensorId == sensor.Id && sensorDimTimes.Year == year && sensorDimTimes.Month == month && sensorDimTimes.Day == day)
      .Select(sensorDimTime => new { sensorDimTimeId = sensorDimTime.Id, hour = sensorDimTime.Hour })
      .ToArrayAsync();
      var xy = new ArrayList();
      foreach (var x in await hours)
      {
        xy.Add(new SensorXyDto(x.hour, await getSensorTotalKwhAnsync(sensor, x.sensorDimTimeId)));
      }
      return xy.Cast<SensorXyDto>().ToArray();
    }
    private Task<float> getSensorTotalKwhAnsync(Sensor sensor, long sensorDimTimeId)
    {
      if (sensor.SensorType == SensorTypes.EnergyLog)
      {
        var wattsTotal = this.SensorEnergyLogs
        .Where(sensorEnergyLog => sensorEnergyLog.SensorDimTimeId == sensorDimTimeId)
        .Select(sensorEnergyLog => sensorEnergyLog.WattsTotal)
        .SumAsync();
        return wattsTotal;
      }
      throw new NotImplementedException();
    }
    public async Task<SensorXyDto[]> GetSensorXyTotalDurationAsync(Sensor sensor, int year, int month, int day)
    {
      var hours = this.SensorDimTimes
      .Where(sensorDimTimes => sensorDimTimes.SensorId == sensor.Id && sensorDimTimes.Year == year && sensorDimTimes.Month == month && sensorDimTimes.Day == day)
      .Select(sensorDimTime => new { sensorDimTimeId = sensorDimTime.Id, hour = sensorDimTime.Hour })
      .ToArrayAsync();
      var xy = new ArrayList();
      foreach (var x in await hours)
      {
        float duratioToHour = await getSensorSumDurationAnsync(sensor, x.sensorDimTimeId);
        if (duratioToHour > 0) duratioToHour = duratioToHour / 3600.00f;
        xy.Add(new SensorXyDto(x.hour, duratioToHour));
      }
      return xy.Cast<SensorXyDto>().ToArray();
    }
    private Task<float> getSensorSumDurationAnsync(Sensor sensor, long sensorDimTimeId)
    {
      if (sensor.SensorType == SensorTypes.EnergyLog)
      {
        var totalDuration = this.SensorEnergyLogs
        .Where(sensorEnergyLog => sensorEnergyLog.SensorDimTimeId == sensorDimTimeId)
        .Select(sensorEnergyLog => sensorEnergyLog.Duration)
        .SumAsync();
        return totalDuration;
      }
      throw new NotImplementedException();
    }
  }
}