using Xunit;
using Backend.Models;

namespace Backend.Test
{
    public class ModelsUnitTests : TestWithSqlite
    {

        [Theory, InlineData("UserWithSensor@sbdia.iot")]
        public Models.ApplicationUser CreateUser(string email)
        {
            var newUser = new Models.ApplicationUser() { Email = email };
            this.DbContext.Add(newUser);
            this.DbContext.SaveChanges();
            Assert.Equal(email, this.DbContext.Users.Find(newUser.Id).Email);
            return newUser;
        }

        [Theory, InlineData("UserWithSensor@sbdia.iot", "My Sensor", SensorTypes.EnergyLog)]
        public Sensor CreateSensor(string userEmail, string sensorName, SensorTypes sensorType)
        {
            var user = CreateUser(userEmail);
            var sensor = new Sensor() { Name = sensorName, SensorType = sensorType, Owner = user };
            this.DbContext.Add(sensor);
            this.DbContext.SaveChanges();
            var userSensors = this.DbContext.Users.Find(user.Id).Sensors;
            Assert.Equal(userSensors.Count, user.Sensors.Count);
            Assert.Equal(userSensors[0].Id, sensor.Id);
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
            var cost = new SensorCost() {Title= title, Sensor= sensor, Value=value};
            this.DbContext.Add(cost);
            this.DbContext.SaveChanges();
            Assert.Equal(sensor.Costs[0], cost );
        }

        [Fact]
        public void CreateSensorCostWithoutTitle()
        {
            Assert.Throws<Microsoft.EntityFrameworkCore.DbUpdateException>(
                () => CreateSensorCost(null, 0.6F)
            );
        }
    }
}
