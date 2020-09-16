using Xunit;
using Backend.Models;
using Backend.Models.Dtos;
using System;
using BackendTests.Mocks;

namespace BackendTest
{
    public class ModelsUnitTests : AppDbContextMock
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
        public Sensor CreateSensor(string userEmail, string sensorName, SensorTypes sensorType)
        {
            //Given
            var user = CreateUser(userEmail);
            var sensor = this.DbContext.CreateSensor(user.Id, new SensorItemDto() { Name = sensorName, SensorType = sensorType });
            //When
            var userSensors = this.DbContext.GetSensors(user.Id);
            //Then
            Assert.NotEmpty(userSensors);
            Assert.Equal(userSensors[0].Id, sensor.Id.ToString());
            return sensor;
        }

        [Fact]
        public void NotCreateSensorWithoutName()
        {
            //Then
            Assert.Throws<Microsoft.EntityFrameworkCore.DbUpdateException>(
                () => CreateSensor("UserWithSensor@sbdia.iot", null, SensorTypes.EnergyLog)
            );
        }

        [Theory, InlineData("$", 0.6F)]
        public void CreateSensorCost(string title, float value)
        {
            //Given
            var sensor = CreateSensor("UserWithSensor@sbdia.iot", "My Sensor", SensorTypes.EnergyLog);
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
            Assert.Throws<Microsoft.EntityFrameworkCore.DbUpdateException>(
                () => CreateSensorCost(null, 0.6F)
            );
        }

        [Theory, InlineData(2020, 7, 17, 12)]
        public void CreateSensorDimTime(int year, int month, int day, int hour)
        {
            //Given
            var sensor = CreateSensor("UserWithSensor@sbdia.iot", "My Sensor", SensorTypes.EnergyLog);
            var sensorCost = this.DbContext.GetLastOrCreateSensorCost(sensor.Id);
            var dateTime = new DateTime(year, month, day, hour, 0, 0);
            var sensorDimTime = new SensorDimTime() { DateTime = dateTime, SensorId = sensor.Id, SensorCostId = sensorCost.Id };
            //When
            this.DbContext.Add(sensorDimTime);
            this.DbContext.SaveChanges();
            //Then
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
            //Then
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
            //Given
            var sensor = CreateSensor("UserWithSensor@sbdia.iot", "My Sensor", SensorTypes.EnergyLog);
            var sensorDimTime = this.DbContext.GetOrCreateSensorDimTime(unixTime, sensor);
            //When
            var savedSensorDimTime = this.DbContext.GetSensorDimTime(sensor.Id, sensorDimTime.Id);
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
        public (Sensor, SensorLogBatch[]) CreateSensorLogBatchEnergyLog(string content)
        {
            //Given
            var sensor = CreateSensor("UserWithSensor@sbdia.iot", "My Sensor", SensorTypes.EnergyLog);
            //When
            var sensorLogBatch = this.DbContext.CreateSensorLogBatch(sensor, content);
            var sensorLogBatchshUnprocessed = this.DbContext.GetSensorLogBatchPending(sensor.Id);
            //Then
            Assert.NotEmpty(sensorLogBatchshUnprocessed);
            Assert.Equal(content, sensorLogBatchshUnprocessed[0].Content);
            Assert.Equal(0, sensorLogBatchshUnprocessed[0].Attempts);
            return (sensor, sensorLogBatchshUnprocessed);
        }

        [Fact]
        public void CreateSensorEnergyLog()
        {
            //Given
            var sensor = CreateSensor("UserWithSensor@sbdia.iot", "My Sensor", SensorTypes.EnergyLog);
            var unixTime = 1574608324;
            var sensorDimTime = this.DbContext.GetOrCreateSensorDimTime(unixTime, sensor);
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
        public void PerformContentSensorLogBatchEnergyLog()
        {
            //Given
            var (sensor, sensorLogBatchshUnprocessed) = this.CreateSensorLogBatchEnergyLog("1574608324;1;2;3");
            //When
            Assert.NotEmpty(sensorLogBatchshUnprocessed);
            this.DbContext.PerformContentSensorLogBatch(sensor);
            sensorLogBatchshUnprocessed = this.DbContext.GetSensorLogBatchPending(sensor.Id);
            var recentEneryLogs = this.DbContext.GetSensorEnergyLogsRecent(sensor);
            //Then
            Assert.Empty(sensorLogBatchshUnprocessed);
            Assert.NotEmpty(recentEneryLogs);
            Assert.Equal(1, recentEneryLogs[0].Id);
            Assert.Equal(sensor.LogDurationMode, recentEneryLogs[0].Duration);
            Assert.Equal(sensor.DefaultToConvert * 1, recentEneryLogs[0].Watts1);
            Assert.Equal(sensor.DefaultToConvert * 2, recentEneryLogs[0].Watts2);
            Assert.Equal(sensor.DefaultToConvert * 3, recentEneryLogs[0].Watts3);
            Assert.Equal(sensor.DefaultToConvert * (1 + 2 + 3), recentEneryLogs[0].WattsTotal);
        }

        [Fact]
        public void CalculateDurationSensorLogBatchEnergyLog()
        {
            //Given
            var log_at_15_20_00 = "1574608800;1;2;3";
            var log_end_line = "|";
            var log_at_15_20_15 = "1574608815;2;3;4";
            var (sensor, sensorLogBatchshUnprocessed) = this.CreateSensorLogBatchEnergyLog
            (
                $"{log_at_15_20_00}{log_end_line}{log_at_15_20_15}"
            );
            //When
            this.DbContext.PerformContentSensorLogBatch(sensor);
            var recentEneryLogs = this.DbContext.GetSensorEnergyLogsRecent(sensor);
            //Then
            Assert.NotEmpty(recentEneryLogs);
            Assert.Equal(2, recentEneryLogs.Length);
            Assert.Equal(sensor.LogDurationMode, recentEneryLogs[1].Duration);
            Assert.Equal(15, recentEneryLogs[0].Duration);
            Assert.Equal((2 + 3 + 4) * sensor.DefaultToConvert, recentEneryLogs[0].WattsTotal);
            Assert.Equal((1 + 2 + 3) * sensor.DefaultToConvert, recentEneryLogs[1].WattsTotal);
        }

        [Fact]
        public void UpdateSensorEnergyLogDurationMode()
        {
            //Given
            var log_at_15_20_00 = "1574608800;1;2;3";
            var log_end_line = "|";
            var log_at_15_20_15 = "1574608815;2;3;4";
            var log_at_15_20_30 = "1574608830;2;3;4";
            var (sensor, sensorLogBatchshUnprocessed) = this.CreateSensorLogBatchEnergyLog
            (
                $"{log_at_15_20_00}{log_end_line}{log_at_15_20_15}{log_end_line}{log_at_15_20_30}"
            );
            //When
            this.DbContext.PerformContentSensorLogBatch(sensor);
            var recentEneryLogs = this.DbContext.GetSensorEnergyLogsRecent(sensor);
            var sensorDto = this.DbContext.GetSensorDto(sensor.Id);
            //Then
            Assert.NotEmpty(recentEneryLogs);
            Assert.Equal(3, recentEneryLogs.Length);
            Assert.Equal(15, sensorDto.LogDurationMode);
        }
    }
}
