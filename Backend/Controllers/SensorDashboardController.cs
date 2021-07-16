using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
  public partial class SensorController : ControllerBase
  {
    [HttpGet("{id}/dashboard/{year:int}/{month:int}/{day:int}")]
    public async Task<ActionResult<Dictionary<string, object>>> GetSensorDashboardAsync(string id, int year, int month, int day)
    {
      var dashboardData = new Dictionary<string, object>();
      var sensor = await this._dbContext.GetSensorAsync(id);
      var xyTotalKwh = await this._dbContext.GetSensorXyTotalKwhAsync(sensor, year, month, day);
      var xyTotalDuration = await this._dbContext.GetSensorXyTotalDurationAsync(sensor, year, month, day);
      var logsRecent = await this._dbContext.GetSensorEnergyLogsRecentAsync(sensor);
      dashboardData.Add("xyTotalKwh", xyTotalKwh);
      dashboardData.Add("xyTotalDuration", xyTotalDuration);
      dashboardData.Add("logsRecent", logsRecent);
      return dashboardData;
    }
  }
}