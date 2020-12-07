
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
    public async Task<SensorXyDto[]> GetSensorXyAsync(Sensor sensor, int year, int month, int day)
    {
      var xn = this.SensorDimTimes
      .Where(sensorDimTimes => sensorDimTimes.SensorId == sensor.Id && sensorDimTimes.Year == year && sensorDimTimes.Month == month && sensorDimTimes.Day == day)
      .Select(sensorDimTime => new { sensorDimTimeId = sensorDimTime.Id, hour = sensorDimTime.Hour })
      .ToArrayAsync();
      var listSensorXyDto = new ArrayList();

      foreach (var x in await xn)
      {
        listSensorXyDto.Add(new SensorXyDto(x.hour, await getSensorYAnsync(sensor, x.sensorDimTimeId)));
      }
      return listSensorXyDto.Cast<SensorXyDto>().ToArray();
    }

    private Task<float> getSensorYAnsync(Sensor sensor, long sensorDimTimeId)
    {
      if (sensor.SensorType == SensorTypes.EnergyLog)
      {
        var y = this.SensorEnergyLogs
        .Where(sensorEnergyLog => sensorEnergyLog.SensorDimTimeId == sensorDimTimeId)
        .Select(sensorEnergyLog => sensorEnergyLog.WattsTotal)
        .SumAsync();
        return y;
      }
      throw new NotImplementedException();
    }
  }
}