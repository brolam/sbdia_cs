
using Xunit;
using Backend.Controllers;
using Backend.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Backend.Models.Dtos;
using BackendTests.Mocks;

namespace BackendTest
{
    public class SensorControllerUnitTests : AppDbContextMock
    {
        private readonly Owner _owner;
        private readonly SensorController _controller;
        private readonly SensorController _controllerAllowAnonymous;

        public SensorControllerUnitTests()
        {
            this._owner = new Owner() { Email = "UserWithSensor@sbdia.iot" };
            base.DbContext.Add(_owner);
            base.DbContext.SaveChanges();
            var claimsUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, this._owner.Id) }, "mock"));
            _controller = new SensorController(this.DbContext);
            _controller.ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() { User = claimsUser } };
            _controllerAllowAnonymous = new SensorController(this.DbContext);
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

        [Fact]
        public async void PostSensorLogBatch()
        {
            //Given
            var createSensor = this.CreateSensor();
            var sensorDto = this.DbContext.GetSensorDto(createSensor.Id);
            var log_at_15_20_00 = "1574608800;1;2;3";
            var log_end_line = "|";
            var log_at_15_20_15 = "1574608815;2;3;4";
            var log_at_15_20_30 = "1574608830;2;3;4";
            var sensorLogBatch = new SensorLogBatchDto()
            {
                SensorId = createSensor.Id,
                SecretApiToken = sensorDto.SecretApiToken,
                Content = $"{log_at_15_20_00}{log_end_line}{log_at_15_20_15}{log_end_line}{log_at_15_20_30}"
            };

            //When
            await this._controllerAllowAnonymous.PostSensorLogBatch(sensorLogBatch);
            var sensorLogBatchs = this.DbContext.GetSensorLogBatchPending(sensorDto.Id);

            //Then
            Assert.NotEmpty(sensorLogBatchs);
            Assert.Equal(sensorLogBatch.Content, sensorLogBatchs[0].Content);
        }

    }
}