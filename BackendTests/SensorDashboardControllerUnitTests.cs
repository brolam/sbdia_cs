
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
      var xy = Assert.IsType<SensorXyDto[]>(dashboardData["xy"]);
      var logsRecent = Assert.IsType<SensorEnergyLogItemDto[]>(dashboardData["logsRecent"]);
      Assert.Equal(15, xy[0].X);
      Assert.Equal(633.072, Math.Round(xy[0].Y, 3));
      Assert.NotEmpty(logsRecent);
    }
  }
}