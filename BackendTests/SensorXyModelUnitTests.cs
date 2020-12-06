using System;
using Xunit;
using Backend.Models.Dtos;
using BackendTests.Mocks;

namespace BackendTest
{
  public partial class ModelsUnitTests : AppDbContextMock
  {
    [Fact]
    public async System.Threading.Tasks.Task GetSensorXyHourAsync()
    {
      //Given
      var (sensor, sensorLogBatchsUnprocessed) = await this.CreateSensorLogBatchEnergyLogAsync
      (
        "1593584095;1;2;3|1593584123;1;2;3|1593584137;1;2;3|1593584151;1;2;3|1593584165;1;2;3|1593584179;1;2;3"
      );
      //When
      this.DbContext.PerformContentSensorLogBatch(sensor);
      int year = 2020, month = 7, day = 1;
      var sensorXyHour = await this.DbContext.GetSensorXyAsync(sensor, year, month, day);
      //Then
      Assert.NotEmpty(sensorXyHour);
      Assert.Equal(6, sensorXyHour[0].X);
      Assert.Equal(949.608, Math.Round(sensorXyHour[0].Y, 3));
    }
  }
}
