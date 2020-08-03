using Xunit;
using Backend.Models;
using System;

namespace Backend.Test
{
    public class ModelsUnitTests : TestWithSqlite
    {

        [Theory, InlineData("UserWithSensor@sbdia.iot")]
        public Models.Owner CreateUser(string email)
        {
            var newUser = new Models.Owner() { Email = email };
            this.DbContext.Add(newUser);
            this.DbContext.SaveChanges();
            Assert.Equal(email, this.DbContext.Users.Find(newUser.Id).Email);
            return newUser;
        }

        [Theory, InlineData("UserWithSensor@sbdia.iot", "My Sensor", SensorTypes.EnergyLog)]
        public Sensor CreateSensor(string userEmail, string sensorName, SensorTypes sensorType)
        {
            var user = CreateUser(userEmail);
            var sensor = this.DbContext.CreateSensor(user.Id, new Models.Dtos.SensorItemDto() { Name = sensorName, SensorType = sensorType });
            var userSensors = this.DbContext.GetSensors(user.Id);
            Assert.NotEmpty(userSensors);
            Assert.Equal(userSensors[0].Id, sensor.Id.ToString());
            return sensor;
        }

        [Fact]
        public void NotCreateSensorWithoutName()
        {
            Assert.Throws<Microsoft.EntityFrameworkCore.DbUpdateException>(
                () => CreateSensor("UserWithSensor@sbdia.iot", null, SensorTypes.EnergyLog)
            );
        }

        [Theory, InlineData("$", 0.6F)]
        public void CreateSensorCost(string title, float value)
        {
            var sensor = CreateSensor("UserWithSensor@sbdia.iot", "My Sensor", SensorTypes.EnergyLog);
            var cost = new SensorCost() { Title = title, SensorId = sensor.Id, Value = value };
            this.DbContext.Add(cost);
            this.DbContext.SaveChanges();
            var savedCost = this.DbContext.GetSensorCost(sensor.Id, cost.Id);
            Assert.Equal(savedCost.Id, cost.Id);
        }

        [Fact]
        public void CreateSensorCostWithoutTitle()
        {
            Assert.Throws<Microsoft.EntityFrameworkCore.DbUpdateException>(
                () => CreateSensorCost(null, 0.6F)
            );
        }

        [Theory, InlineData(2020, 7, 17, 12)]
        public void CreateSensorDimTime(int year, int month, int day, int hour)
        {
            var sensor = CreateSensor("UserWithSensor@sbdia.iot", "My Sensor", SensorTypes.EnergyLog);
            var sensorCost = this.DbContext.GetLastOrCreateSensorCost(sensor.Id);
            var dateTime = new DateTime(year, month, day, hour, 0, 0);
            var sensorDimTime = new SensorDimTime() { DateTime = dateTime, SensorId = sensor.Id, SensorCostId = sensorCost.Id };
            this.DbContext.Add(sensorDimTime);
            this.DbContext.SaveChanges();
            var savedSensorDimTime = this.DbContext.GetSensorDimTime(sensor.Id, sensorDimTime.Id);
            Assert.Equal(year, savedSensorDimTime.Year);
            Assert.Equal(month, savedSensorDimTime.Month);
            Assert.Equal(day, savedSensorDimTime.Day);
            Assert.Equal(DayOfWeek.Friday, savedSensorDimTime.DayOfWeek);
            Assert.Equal(PeriodOfDayTypes.Morning, savedSensorDimTime.PeriodOfDay);
        }

        [Fact]
        public void SensorDimTimeGetPeriodOfDayFromHour()
        {
            Assert.Equal(PeriodOfDayTypes.EarlyMorning, SensorDimTime.GetPeriodOfDayFromHour(5));
            Assert.Equal(PeriodOfDayTypes.Morning, SensorDimTime.GetPeriodOfDayFromHour(9));
            Assert.Equal(PeriodOfDayTypes.Noon, SensorDimTime.GetPeriodOfDayFromHour(13));
            Assert.Equal(PeriodOfDayTypes.Eve, SensorDimTime.GetPeriodOfDayFromHour(17));
            Assert.Equal(PeriodOfDayTypes.Night, SensorDimTime.GetPeriodOfDayFromHour(21));
            Assert.Equal(PeriodOfDayTypes.LateNight, SensorDimTime.GetPeriodOfDayFromHour(3));
        }

        [Theory, InlineData(1595288285, 2020, 07, 20, 20, DayOfWeek.Monday, PeriodOfDayTypes.Eve)]
        public void CreateSensorDimTimeFromUnixTime(long unixTime, int year, int month, int day, int hour, DayOfWeek dayOfWeek, PeriodOfDayTypes periodOfDayType)
        {
            var sensor = CreateSensor("UserWithSensor@sbdia.iot", "My Sensor", SensorTypes.EnergyLog);
            var sensorDimTime = this.DbContext.GetOrCreateSensorDimTime(unixTime, sensor);
            var savedSensorDimTime = this.DbContext.GetSensorDimTime(sensor.Id, sensorDimTime.Id);
            Assert.Equal(year, savedSensorDimTime.Year);
            Assert.Equal(month, savedSensorDimTime.Month);
            Assert.Equal(day, savedSensorDimTime.Day);
            Assert.Equal(hour, savedSensorDimTime.Hour);
            Assert.Equal(dayOfWeek, savedSensorDimTime.DayOfWeek);
            Assert.Equal(periodOfDayType, savedSensorDimTime.PeriodOfDay);
        }

    }
}
