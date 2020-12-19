using System;
using Xunit;
using Backend.Models.Dtos;
using BackendTests.Mocks;
using System.Threading.Tasks;

namespace BackendTest
{
  public partial class ModelsUnitTests : AppDbContextMock
  {
    [Fact]
    public async Task GetSensorXyTotalKwhAsync()
    {
      //Given
      var (sensor, sensorLogBatchsUnprocessed) = await this.CreateSensorLogBatchEnergyLogAsync
      (
        "1593584095;1;2;3|1593584123;1;2;3|1593584137;1;2;3|1593584151;1;2;3|1593584165;1;2;3|1593584179;1;2;3"
      );
      //When
      this.DbContext.PerformContentSensorLogBatch(sensor);
      int year = 2020, month = 7, day = 1;
      var sensorXyHour = await this.DbContext.GetSensorXyTotalKwhAsync(sensor, year, month, day);
      //Then
      Assert.NotEmpty(sensorXyHour);
      Assert.Equal(6, sensorXyHour[0].X);
      Assert.Equal(0.004, Math.Round(sensorXyHour[0].Y, 3));
    }
    [Fact]
    public async Task GetSensorXyTotalDurationAsync()
    {
      //Given
      var (sensor, sensorLogBatchsUnprocessed) = await this.CreateSensorLogBatchEnergyLogAsync
      (
        "1593584095;1;2;3|1593584123;1;2;3|1593584137;1;2;3|1593584151;1;2;3|1593584165;1;2;3|1593584179;1;2;3"
      );
      //When
      this.DbContext.PerformContentSensorLogBatch(sensor);
      int year = 2020, month = 7, day = 1;
      var sensorXyHour = await this.DbContext.GetSensorXyTotalDurationAsync(sensor, year, month, day);
      //Then
      Assert.NotEmpty(sensorXyHour);
      Assert.Equal(6, sensorXyHour[0].X);
      Assert.Equal(0.027, Math.Round(sensorXyHour[0].Y, 3));
    }
    [Fact]
    public async Task GetSensorXyDaysAsync()
    {
      //Given
      var (sensor, sensorLogBatchsUnprocessed) = await this.CreateSensorLogBatchEnergyLogAsync
      (
        "1606780800;1;2;3|1606867200;1;2;3|1606953600;1;2;3|1607040000;1;2;3|1607126400;1;2;3|1607212800;1;2;3"
      );
      //When
      this.DbContext.PerformContentSensorLogBatch(sensor);
      var sensorXyDays = await this.DbContext.GetSensorXyDaysAsync(sensor);
      //Then
      Assert.NotEmpty(sensorXyDays);
      Assert.Equal("2020/12/01", sensorXyDays[0]);
      Assert.Equal("2020/12/02", sensorXyDays[1]);
      Assert.Equal("2020/12/03", sensorXyDays[2]);
      Assert.Equal("2020/12/04", sensorXyDays[3]);
      Assert.Equal("2020/12/05", sensorXyDays[4]);
      Assert.Equal("2020/12/06", sensorXyDays[5]);
    }
  }
}
