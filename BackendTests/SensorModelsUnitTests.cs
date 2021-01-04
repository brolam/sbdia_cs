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
