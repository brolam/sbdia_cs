using Xunit;
using Backend.Models;
using Backend.Models.Dtos;
using BackendTests.Mocks;
using System.Threading.Tasks;

namespace BackendTest
{
  public partial class ModelsUnitTests : AppDbContextMock
  {
    [Theory]
    [InlineData("1574608324;1;2;3")]
    public async Task<(Sensor, SensorLogBatch[])> CreateSensorLogBatchEnergyLogAsync(string content)
    {
      //Given
      var sensor = await CreateSensorAsync("UserWithSensor@sbdia.iot", "My Sensor", SensorTypes.EnergyLog);
      //When
      var sensorLogBatch = this.DbContext.CreateSensorLogBatch(sensor, content);
      var sensorLogBatchsUnprocessed = this.DbContext.GetSensorLogBatchPending(sensor.Id);
      //Then
      Assert.NotEmpty(sensorLogBatchsUnprocessed);
      Assert.Equal(content, sensorLogBatchsUnprocessed[0].Content);
      Assert.Equal(0, sensorLogBatchsUnprocessed[0].Attempts);
      return (sensor, sensorLogBatchsUnprocessed);
    }

    [Fact]
    public async Task CreateSensorEnergyLogAsync()
    {
      //Given
      var sensor = await CreateSensorAsync("UserWithSensor@sbdia.iot", "My Sensor", SensorTypes.EnergyLog);
      var unixTime = 1574608324;
      var sensorDimTime = this.DbContext.GetOrCreateSensorDimTime(sensor, unixTime);
      const float Duration = 14.00F;
      const float Watts1 = 1.00F;
      const float Watts2 = 2.00F;
      const float Watts3 = 3.00F;
      const float ConvertToUnits = 0.60F;
      //When
      var sensorEnergyLog = this.DbContext.CreateSensorEnergyLog(sensor.Id, sensorDimTime.Id, unixTime, Duration, Watts1, Watts2, Watts3, ConvertToUnits);
      //Then
      Assert.True(sensorEnergyLog.Id > 0);
      Assert.Equal(sensor.Id, sensorEnergyLog.SensorId);
      Assert.Equal(sensorDimTime.Id, sensorEnergyLog.SensorDimTimeId);
      Assert.Equal(Duration, sensorEnergyLog.Duration);
      Assert.Equal(Watts1, sensorEnergyLog.Watts1);
      Assert.Equal(Watts2, sensorEnergyLog.Watts2);
      Assert.Equal(Watts3, sensorEnergyLog.Watts3);
      Assert.Equal(ConvertToUnits, sensorEnergyLog.ConvertToUnits);
    }

    [Fact]
    public async Task PerformContentSensorLogBatchEnergyLogAsync()
    {
      //Given
      var (sensor, sensorLogBatchsUnprocessed) = await this.CreateSensorLogBatchEnergyLogAsync("1574608324;1;2;3");
      //When
      Assert.NotEmpty(sensorLogBatchsUnprocessed);
      this.DbContext.PerformContentSensorLogBatch(sensor);
      sensorLogBatchsUnprocessed = this.DbContext.GetSensorLogBatchPending(sensor.Id);
      var recentEneryLogs = await this.DbContext.GetSensorEnergyLogsRecentAsync(sensor);
      //Then
      Assert.Empty(sensorLogBatchsUnprocessed);
      Assert.NotEmpty(recentEneryLogs);
      Assert.Equal(1, recentEneryLogs[0].Id);
      Assert.Equal(sensor.LogDurationMode, recentEneryLogs[0].Duration);
      Assert.Equal(sensor.DefaultToConvert * 1, recentEneryLogs[0].Watts1);
      Assert.Equal(sensor.DefaultToConvert * 2, recentEneryLogs[0].Watts2);
      Assert.Equal(sensor.DefaultToConvert * 3, recentEneryLogs[0].Watts3);
      Assert.Equal(sensor.DefaultToConvert * (1 + 2 + 3), recentEneryLogs[0].WattsTotal);
    }

    [Fact]
    public async Task PerformContentSensorLogBatchEnergyLogSetAttemptsAsync()
    {
      //Given
      var (sensor, sensorLogBatchsWithError) = await this.CreateSensorLogBatchEnergyLogAsync("1574608324;1;2");

      //When
      Assert.NotEmpty(sensorLogBatchsWithError);
      var attempts = 2;
      for (int attempt = 1; attempt <= attempts; attempt++)
      {
        this.DbContext.PerformContentSensorLogBatch(sensor);
      }
      var sensorLogBatchsUnprocessed = this.DbContext.GetSensorLogBatchPending(sensor.Id);

      //Then
      Assert.NotEmpty(sensorLogBatchsUnprocessed);
      Assert.Equal(attempts, sensorLogBatchsUnprocessed[0].Attempts);
      Assert.NotEmpty(sensorLogBatchsUnprocessed[0].Exception);
    }

    [Fact]
    public async Task PerformContentSensorLogBatchEnergyLogSetMaxAttemptsAsync()
    {
      //Given
      var (sensor, sensorLogBatchsWithError) = await this.CreateSensorLogBatchEnergyLogAsync("1574608324;1;2");

      //When
      Assert.NotEmpty(sensorLogBatchsWithError);
      var attempts = 4;
      for (int attempt = 1; attempt <= attempts; attempt++)
      {
        this.DbContext.PerformContentSensorLogBatch(sensor);
      }
      var sensorLogBatchsUnprocessed = this.DbContext.GetSensorLogBatchPending(sensor.Id);

      //Then
      Assert.Empty(sensorLogBatchsUnprocessed);
    }

    [Fact]
    public async void PerformContentSensorLogBatchEnergyLogUpdateIfExists()
    {
      //Given
      var (sensor, sensorLogBatchsWithError) = await this.CreateSensorLogBatchEnergyLogAsync("1574608324;1;2;3");

      //When
      await this.DbContext.CreateSensorLogBatch(sensor, "1574608324;4;5;6");
      var sensorLogBatchsUnprocessed = this.DbContext.GetSensorLogBatchPending(sensor.Id);
      Assert.Equal(2, sensorLogBatchsUnprocessed.Length);
      this.DbContext.PerformContentSensorLogBatch(sensor);
      var recentEneryLogs = await this.DbContext.GetSensorEnergyLogsRecentAsync(sensor);

      //Then
      Assert.NotEmpty(recentEneryLogs);
      Assert.Equal(1, (int)recentEneryLogs.Length);
      Assert.Equal(sensor.DefaultToConvert * 4, recentEneryLogs[0].Watts1);
      Assert.Equal(sensor.DefaultToConvert * 5, recentEneryLogs[0].Watts2);
      Assert.Equal(sensor.DefaultToConvert * 6, recentEneryLogs[0].Watts3);
      Assert.Equal(sensor.DefaultToConvert * (4 + 5 + 6), recentEneryLogs[0].WattsTotal);

    }

    [Fact]
    public async Task CalculateDurationSensorLogBatchEnergyLogAsync()
    {
      //Given
      var log_at_15_20_00 = "1574608800;1;2;3";
      var log_end_line = "|";
      var log_at_15_20_15 = "1574608815;2;3;4";
      var (sensor, sensorLogBatchsUnprocessed) = await this.CreateSensorLogBatchEnergyLogAsync
      (
          $"{log_at_15_20_00}{log_end_line}{log_at_15_20_15}"
      );
      //When
      this.DbContext.PerformContentSensorLogBatch(sensor);
      var recentEneryLogs = await this.DbContext.GetSensorEnergyLogsRecentAsync(sensor);
      //Then
      Assert.NotEmpty(recentEneryLogs);
      Assert.Equal(2, recentEneryLogs.Length);
      Assert.Equal(14, recentEneryLogs[1].Duration);
      Assert.Equal(15, recentEneryLogs[0].Duration);
      Assert.Equal((2 + 3 + 4) * sensor.DefaultToConvert, recentEneryLogs[0].WattsTotal);
      Assert.Equal((1 + 2 + 3) * sensor.DefaultToConvert, recentEneryLogs[1].WattsTotal);
    }

    [Fact]
    public async Task UpdateSensorEnergyLogDurationModeAsync()
    {
      //Given
      var log_at_15_20_00 = "1574608800;1;2;3";
      var log_end_line = "|";
      var log_at_15_20_15 = "1574608815;2;3;4";
      var log_at_15_20_30 = "1574608830;2;3;4";
      var (sensor, sensorLogBatchsUnprocessed) = await this.CreateSensorLogBatchEnergyLogAsync
      (
          $"{log_at_15_20_00}{log_end_line}{log_at_15_20_15}{log_end_line}{log_at_15_20_30}"
      );
      //When
      this.DbContext.PerformContentSensorLogBatch(sensor);
      var recentEneryLogs = await this.DbContext.GetSensorEnergyLogsRecentAsync(sensor);
      var sensorDto = this.DbContext.GetSensorDto(sensor.Id);
      //Then
      Assert.NotEmpty(recentEneryLogs);
      Assert.Equal(3, recentEneryLogs.Length);
      Assert.Equal(15, sensorDto.LogDurationMode);
    }

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
    [Fact]
    public async void GetSensorEnergyLogLast()
    {
      //Given
      var (sensor, sensorLogBatchsUnprocessed) = await this.CreateSensorLogBatchEnergyLogAsync
      (
        "1593584095;1;2;3|1593584123;1;2;3"
      );
      //When
      this.DbContext.PerformContentSensorLogBatch(sensor);
      var energyLogLast = this.DbContext.GetSensorEnergyLogLast(sensor);
      //Then
      Assert.Equal(1593584123, energyLogLast.UnixTime);
    }
  }
}
