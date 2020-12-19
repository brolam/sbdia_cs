
using System;
using Backend.Models;
using Backend.Models.Dtos;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;

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
        float totalWatts = await getSensorTotalWattsAnsync(sensor, x.sensorDimTimeId);
        float totalKwh = totalWatts > 0 ? totalWatts / 3600.00f / 1000.00f : 0.00f;
        xy.Add(new SensorXyDto(x.hour, totalKwh));
      }
      return xy.Cast<SensorXyDto>().ToArray();
    }
    private Task<float> getSensorTotalWattsAnsync(Sensor sensor, long sensorDimTimeId)
    {
      if (sensor.SensorType == SensorTypes.EnergyLog)
      {
        var wattsTotal = this.SensorEnergyLogs
        .Where(sensorEnergyLog => sensorEnergyLog.SensorDimTimeId == sensorDimTimeId)
        .Select(sensorEnergyLog => sensorEnergyLog.WattsTotal * sensorEnergyLog.Duration)
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
    public Task<string[]> GetSensorXyDaysAsync(Sensor sensor)
    {
      var days = this.SensorDimTimes
      .Where(sensorDimTimes => sensorDimTimes.SensorId == sensor.Id)
      .Select(sensorDimTime => $"{sensorDimTime.Year}/{sensorDimTime.Month:00}/{sensorDimTime.Day:00}")
      .Distinct();
      return days.ToArrayAsync();
    }
  }
}