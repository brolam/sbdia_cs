
using Xunit;
using BackendTests.Mocks;
using System.Threading.Tasks;
using System;

namespace BackendTest
{
  public partial class SensorControllerUnitTests : AppDbContextMock
  {

    [Fact]
    public async Task GetSensorXyAsync()
    {
      // Act
      this.PostSensorLogBatch();
      var sensorsDto = this.DbContext.GetSensors(this._owner.Id).Result;
      var sensor = this.DbContext.GetSensorAsync(sensorsDto[0].Id).Result;
      //When
      this.DbContext.PerformContentSensorLogBatch(sensor);
      var actionResult = await _controller.GetSensorXyAsync(sensor.Id, 2019, 11, 24);
      // Assert
      Assert.NotEmpty(actionResult.Value);
      Assert.Equal(15, actionResult.Value[0].X);
      Assert.Equal(633.072, Math.Round(actionResult.Value[0].Y, 3));
    }
  }
}