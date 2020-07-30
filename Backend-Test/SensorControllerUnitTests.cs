

using System.Net;
using System.Threading.Tasks;
using Xunit;
using Backend.Controllers;
using Backend.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Backend.Models.Dtos;

namespace Backend.Test
{

    public class SensorControllerUnitTests : TestWithSqlite
    {
        private readonly ApplicationUser _user;
        private readonly SensorController _controller;
        public SensorControllerUnitTests()
        {
            _user = new Models.ApplicationUser() { Email = "UserWithSensor@sbdia.iot" };
            base.DbContext.Add(_user);
            base.DbContext.SaveChanges();
            var claimsUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, this._user.Id) }, "mock"));
            _controller = new SensorController(this.DbContext);
            _controller.ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() { User = claimsUser } };
        }

        [Fact]
        public void GetSensorEmptyList()
        {
            // Act
            var actionResult = _controller.GetSensorAll();
            // Assert
            Assert.Null(actionResult.Result);
        }

        [Fact]
        public SensorItemDto CreateSensor()
        {
            // Act
            var sensorItemDto = new SensorItemDto() { Name = "Sensor 01", SensorType = SensorTypes.EnergyLog };
            var actionResult = _controller.CreateSensor(sensorItemDto);
            var sensorItemDtoResult = Assert.IsType<SensorItemDto>(((CreatedAtActionResult)actionResult.Result).Value);
            // Assert
            Assert.NotEmpty(sensorItemDtoResult.Id);
            Assert.Equal(sensorItemDto.Name, sensorItemDtoResult.Name);
            return sensorItemDtoResult;
        }

        [Fact]
        public void GetSensor()
        {
            //Act
            var createSensor = this.CreateSensor();
            var actionResult = _controller.GetSensor(createSensor.Id);
            //Assert
            Assert.Equal(createSensor.Id, actionResult.Value.Id);
        }
    }
}