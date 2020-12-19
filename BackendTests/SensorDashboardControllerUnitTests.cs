
using Xunit;
using BackendTests.Mocks;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Backend.Models.Dtos;

namespace BackendTest
{
  public partial class SensorControllerUnitTests : AppDbContextMock
  {
    [Fact]
    public async Task GetSensorDashboardAsync()
    {
      // Act
      this.PostSensorLogBatch();
      var sensorsDto = this.DbContext.GetSensors(this._owner.Id).Result;
      var sensor = this.DbContext.GetSensorAsync(sensorsDto[0].Id).Result;
      //When
      this.DbContext.PerformContentSensorLogBatch(sensor);
      var actionResult = await _controller.GetSensorDashboardAsync(sensor.Id, 2019, 11, 24);
      // Assert
      Assert.NotEmpty(actionResult.Value);
      var dashboardData = Assert.IsType<Dictionary<string, object>>(actionResult.Value);
      var xyTotalKwh = Assert.IsType<SensorXyDto[]>(dashboardData["xyTotalKwh"]);
      var xyTotalDuration = Assert.IsType<SensorXyDto[]>(dashboardData["xyTotalDuration"]);
      var logsRecent = Assert.IsType<SensorEnergyLogItemDto[]>(dashboardData["logsRecent"]);
      var xyDays = Assert.IsType<string[]>(dashboardData["xyDays"]);
      Assert.NotEmpty(xyTotalKwh);
      Assert.NotEmpty(xyTotalDuration);
      Assert.NotEmpty(logsRecent);
      Assert.NotEmpty(xyDays);
      Assert.Equal(15, xyTotalKwh[0].X);
      Assert.Equal(0.003, Math.Round(xyTotalKwh[0].Y, 3));
    }
  }
}