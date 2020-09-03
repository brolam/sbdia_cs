

using System.Net;
using System.Threading.Tasks;
using Xunit;
using Backend.Controllers;
using Backend.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Backend.Models.Dtos;

namespace BackendTest
{

    public class SensorControllerUnitTests : TestWithSqlite
    {
        private readonly Owner _owner;
        private readonly SensorController _controller;
        public SensorControllerUnitTests()
        {
            this._owner = new Models.Owner() { Email = "UserWithSensor@sbdia.iot" };
            base.DbContext.Add(_owner);
            base.DbContext.SaveChanges();
            var claimsUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, this._owner.Id) }, "mock"));
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