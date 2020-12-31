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
    public async Task CreateSensorEnergyLogWithDurationBetweenTwentyPercentLogDurationMode()
    {
      //Given
      var (sensor, sensorLogBatchsUnprocessed) = await this.CreateSensorLogBatchEnergyLogAsync
      (
        "1593584123;1;2;3|1593584095;1;2;3"
      );
      //When
      this.DbContext.PerformContentSensorLogBatch(sensor);
      SensorEnergyLogItemDto[] sensorEnergyLogs = await this.DbContext.GetSensorEnergyLogsRecentAsync(sensor);
      //Then
      Assert.NotEmpty(sensorEnergyLogs);
      Assert.Equal(14, sensorEnergyLogs[0].Duration);
      Assert.Equal(14, sensorEnergyLogs[1].Duration);
    }

  }
}
