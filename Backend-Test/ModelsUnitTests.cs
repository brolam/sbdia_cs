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
        public void CreateSensor(string userEmail, string sensorName, SensorTypes sensorType)
        {
            var user = CreateUser(userEmail);
            var sensor = new Sensor() { Name = sensorName, SensorType = sensorType, Owner = user };
            this.DbContext.Add(sensor);
            this.DbContext.SaveChanges();
            var userSensors = this.DbContext.Users.Find(user.Id).Sensors;
            Assert.Equal(userSensors.Count, user.Sensors.Count);
            Assert.Equal(userSensors[0].Id, sensor.Id);
        }

        [Fact]
        public void NotCreateSensorWithoutName()
        {
            Assert.Throws<Microsoft.EntityFrameworkCore.DbUpdateException>(
                () => CreateSensor("UserWithSensor@sbdia.iot", null, SensorTypes.EnergyLog)
            );
        }
    }
}
