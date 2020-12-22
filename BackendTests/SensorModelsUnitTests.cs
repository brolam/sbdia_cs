using Xunit;
using Backend.Models;
using Backend.Models.Dtos;
using System;
using BackendTests.Mocks;
using System.Threading.Tasks;

namespace BackendTest
{
  public partial class ModelsUnitTests : AppDbContextMock
  {

    [Theory, InlineData("UserWithSensor@sbdia.iot")]
    public Owner CreateUser(string email)
    {
      //Given
      var newUser = new Owner() { Email = email };
      //When
      this.DbContext.Add(newUser);
      this.DbContext.SaveChanges();
      //Then
      Assert.Equal(email, this.DbContext.Users.Find(newUser.Id).Email);
      return newUser;
    }

    [Theory, InlineData("UserWithSensor@sbdia.iot", "My Sensor", SensorTypes.EnergyLog)]
    public async Task<Sensor> CreateSensorAsync(string userEmail, string sensorName, SensorTypes sensorType)
    {
      //Given
      var user = CreateUser(userEmail);
      var sensor = this.DbContext.CreateSensor(user.Id, new SensorItemDto() { Name = sensorName, SensorType = sensorType, TimeZone = TimeZoneInfo.Local.Id });
      //When
      var userSensors = await this.DbContext.GetSensors(user.Id);
      //Then
      Assert.NotEmpty(userSensors);
      Assert.Equal(sensor.Id.ToString(), userSensors[0].Id);
      Assert.Equal("My Sensor", userSensors[0].Name);
      Assert.Equal(SensorTypes.EnergyLog, userSensors[0].SensorType);
      Assert.Equal("EnergyLog", userSensors[0].SensorTypeName);
      Assert.Equal(14, userSensors[0].LogDurationMode);
      Assert.Equal(new DateTime(2020, 10, 11, 12, 55, 59), userSensors[0].LogLastRecorded);
      return sensor;
    }

    [Fact]
    public void NotCreateSensorWithoutName()
    {
      //Then
      Assert.ThrowsAsync<Microsoft.EntityFrameworkCore.DbUpdateException>(
          () => CreateSensorAsync("UserWithSensor@sbdia.iot", null, SensorTypes.EnergyLog)
      );
    }

    [Theory, InlineData("$", 0.6F)]
    public async Task CreateSensorCostAsync(string title, float value)
    {
      //Given
      var sensor = await CreateSensorAsync("UserWithSensor@sbdia.iot", "My Sensor", SensorTypes.EnergyLog);
      var cost = new SensorCost() { Title = title, SensorId = sensor.Id, Value = value };
      //When
      this.DbContext.Add(cost);
      this.DbContext.SaveChanges();
      var savedCost = this.DbContext.GetSensorCost(sensor.Id, cost.Id);
      //Then
      Assert.Equal(savedCost.Id, cost.Id);
    }

    [Fact]
    public void CreateSensorCostWithoutTitle()
    {
      //Then
      Assert.ThrowsAsync<Microsoft.EntityFrameworkCore.DbUpdateException>(
          () => CreateSensorCostAsync(null, 0.6F)
      );
    }

    [Theory, InlineData(2020, 7, 17, 12)]
    public async Task CreateSensorDimTimeAsync(int year, int month, int day, int hour)
    {
      //Given
      var sensor = await CreateSensorAsync("UserWithSensor@sbdia.iot", "My Sensor", SensorTypes.EnergyLog);
      var sensorCost = this.DbContext.GetLastOrCreateSensorCost(sensor.Id);
      var dateTime = new DateTimeOffset(year, month, day, hour, 0, 0, 0, TimeSpan.Zero);
      //When
      var sensorDimTimeCreated = this.DbContext.GetOrCreateSensorDimTime(sensor, dateTime.ToUnixTimeSeconds());
      var sensorDimTimeGot = this.DbContext.GetOrCreateSensorDimTime(sensor, dateTime.ToUnixTimeSeconds());
      //Then
      Assert.Equal(year, sensorDimTimeCreated.Year);
      Assert.Equal(month, sensorDimTimeCreated.Month);
      Assert.Equal(day, sensorDimTimeCreated.Day);
      Assert.Equal(DayOfWeek.Friday, sensorDimTimeCreated.DayOfWeek);
      Assert.Equal(PeriodOfDayTypes.Morning, sensorDimTimeCreated.PeriodOfDay);
      Assert.Equal(sensorDimTimeCreated.Id, sensorDimTimeGot.Id);
    }

    [Fact]
    public void SensorDimTimeGetPeriodOfDayFromHour()
    {
      //Then
      Assert.Equal(PeriodOfDayTypes.EarlyMorning, SensorDimTime.GetPeriodOfDayFromHour(5));
      Assert.Equal(PeriodOfDayTypes.Morning, SensorDimTime.GetPeriodOfDayFromHour(9));
      Assert.Equal(PeriodOfDayTypes.Noon, SensorDimTime.GetPeriodOfDayFromHour(13));
      Assert.Equal(PeriodOfDayTypes.Eve, SensorDimTime.GetPeriodOfDayFromHour(17));
      Assert.Equal(PeriodOfDayTypes.Night, SensorDimTime.GetPeriodOfDayFromHour(21));
      Assert.Equal(PeriodOfDayTypes.LateNight, SensorDimTime.GetPeriodOfDayFromHour(3));
    }

    [Theory, InlineData(1595288285, 2020, 07, 20, 23, DayOfWeek.Monday, PeriodOfDayTypes.Night)]
    public async Task CreateSensorDimTimeFromUnixTimeAsync(long unixTime, int year, int month, int day, int hour, DayOfWeek dayOfWeek, PeriodOfDayTypes periodOfDayType)
    {
      //Given
      var sensor = await CreateSensorAsync("UserWithSensor@sbdia.iot", "My Sensor", SensorTypes.EnergyLog);
      var sensorDimTime = this.DbContext.GetOrCreateSensorDimTime(sensor, unixTime);
      //When
      var savedSensorDimTime = this.DbContext.GetSensorDimTime(sensor.Id, sensorDimTime.DateTime);
      //Then
      Assert.Equal(year, savedSensorDimTime.Year);
      Assert.Equal(month, savedSensorDimTime.Month);
      Assert.Equal(day, savedSensorDimTime.Day);
      Assert.Equal(hour, savedSensorDimTime.Hour);
      Assert.Equal(dayOfWeek, savedSensorDimTime.DayOfWeek);
      Assert.Equal(periodOfDayType, savedSensorDimTime.PeriodOfDay);
    }

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
    public async void GetSensorWithLogBatchPending()
    {
      //Given
      var (sensor, sensorLogBatchs) = await CreateSensorLogBatchEnergyLogAsync("1574608324;1;2;3");
      var sensorWithLogBatchPending = await this.DbContext.GetSensorWithLogBatchPending();
      //Then
      Assert.NotEmpty(sensorWithLogBatchPending);
      Assert.Equal(sensor.Id, sensorWithLogBatchPending[0].Id);
    }

    [Fact]
    public async void GetSensorWithoutLogBatchPending()
    {
      //Given
      var (sensor, sensorLogBatchs) = await CreateSensorLogBatchEnergyLogAsync("1574608324;1;2;3");
      //When
      this.DbContext.PerformContentSensorLogBatch(sensor);
      var sensorWithoutLogBatchPending = await this.DbContext.GetSensorWithLogBatchPending();
      //Then
      Assert.Empty(sensorWithoutLogBatchPending);
    }
  }
}
