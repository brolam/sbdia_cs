
using Xunit;
using Backend.Controllers;
using Backend.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Backend.Models.Dtos;
using BackendTests.Mocks;
using System;
using System.Text;

namespace BackendTest
{
  public partial class SensorControllerUnitTests : AppDbContextMock
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
      Assert.Empty(actionResult.Result.Value);
    }

    [Fact]
    public SensorItemDto CreateSensor()
    {
      // Act
      var sensorItemDto = new SensorItemDto() { Name = "Sensor 01", SensorType = SensorTypes.EnergyLog, TimeZone = System.TimeZoneInfo.Local.Id };
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
      var sensorId = sensorDto.Id;
      var secretApiToken = sensorDto.SecretApiToken;
      var content = $"{log_at_15_20_00}{log_end_line}{log_at_15_20_15}{log_end_line}{log_at_15_20_30}";
      //When
      var result = await this._controllerAllowAnonymous.PostSensorLogBatch(
        sensorId,
        secretApiToken,
        content
      );
      var resultValue = Assert.IsType<ObjectResult>(((ObjectResult)result));
      var sensorLogBatchs = this.DbContext.GetSensorLogBatchPending(sensorDto.Id);

      //Then
      Assert.Equal(201, resultValue.StatusCode);
      Assert.NotEmpty(sensorLogBatchs);
      Assert.Equal(content, sensorLogBatchs[0].Content);
    }

    [Fact]
    public async void PostSensorLogBatchNotFoundSensor()
    {
      //Given
      var createSensor = this.CreateSensor();
      var sensorDto = this.DbContext.GetSensorDto(createSensor.Id);
      var log_at_15_20_00 = "1574608800;1;2;3";
      var sensorIdInvalid = "bd6cb0ca-24a1-4064-97f8-b95f3cfeb1cf";
      var sensorId = sensorIdInvalid;
      var secretApiToken = sensorDto.SecretApiToken;
      var content = $"{log_at_15_20_00}";
      //When
      var result = await this._controllerAllowAnonymous.PostSensorLogBatch
      (
          sensorId,
          secretApiToken,
          content
      );

      //Then
      Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async void PostSensorLogBatchNotFoundSecretApiToken()
    {
      //Given
      var createSensor = this.CreateSensor();
      var sensorDto = this.DbContext.GetSensorDto(createSensor.Id);
      var log_at_15_20_00 = "1574608800;1;2;3";
      var sensorId = sensorDto.Id;
      var secretApiTokenInvalid = "bd6cb0ca-24a1-4064-97f8-b95f3cfeb1cf";
      var content = $"{log_at_15_20_00}";
      //When
      var result = await this._controllerAllowAnonymous.PostSensorLogBatch
      (
          sensorId,
          secretApiTokenInvalid,
          content
      );
      //Then
      Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void GetSensorTimeZones()
    {
      //Given
      var actionResult = this._controller.GetSensorTimeZones();
      //When
      var timeZones = Assert.IsType<string[]>(actionResult.Value);
      //Then
      Assert.NotEmpty(timeZones);
    }

    [Fact]
    public async void GetSensorLogsToCsv()
    {
      //Given
      var createSensor = this.CreateSensor();
      var sensorDto = this.DbContext.GetSensorDto(createSensor.Id);
      var log_at_15_20_00 = "1574608800;1;2;3";
      var log_end_line = "|";
      var log_at_15_20_15 = "1574608815;2;3;4";
      var log_at_15_20_30 = "1574608830;2;3;4";
      var sensorId = sensorDto.Id;
      var secretApiToken = sensorDto.SecretApiToken;
      var content = $"{log_at_15_20_00}{log_end_line}{log_at_15_20_15}{log_end_line}{log_at_15_20_30}";
      //When
      await this._controllerAllowAnonymous.PostSensorLogBatch(
        sensorId,
        secretApiToken,
        content
      );
      var sensor = await this.DbContext.GetSensorAsync(sensorDto.Id);
      this.DbContext.PerformContentSensorLogBatch(sensor);
      var result = await this._controllerAllowAnonymous.GetSensorLogsToCsv(sensorDto.Id, 2019, 11, sensorDto.SecretApiToken);
      //Then
      var resultValue = Assert.IsType<FileContentResult>(result);
      Assert.NotEmpty(resultValue.FileContents);
      var rows = Encoding.UTF8.GetString(resultValue.FileContents).Split(Environment.NewLine);
      Assert.Equal("Day,Hour,PeriodOfDay,DayOfWeek,UnixTime,Duration,Watts1,Watts2,Watts3,WattsTotal", rows[0]);
      Assert.Equal("24,15,Noon,Sunday,1574608800,14,26.378,52.756,79.134,158.268", rows[1]);
      Assert.Equal("24,15,Noon,Sunday,1574608815,15,52.756,79.134,105.512,237.40201", rows[2]);
      Assert.Equal("24,15,Noon,Sunday,1574608830,15,52.756,79.134,105.512,237.40201", rows[3]);
    }

    [Fact]
    public async void GetSensorLogsToCsvNotFoundSensor()
    {
      //Given
      var createSensor = this.CreateSensor();
      //When
      var result = await this._controllerAllowAnonymous.GetSensorLogsToCsv("INVALID ID", 2020, 1, createSensor.SecretApiToken);
      //Then
      Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async void GetSensorLogsToCsvNotFoundSecretApiToken()
    {
      //Given
      var createSensor = this.CreateSensor();
      //When
      var result = await this._controllerAllowAnonymous.GetSensorLogsToCsv(createSensor.Id, 2020, 1, "INVALID SecretApiToken");
      //Then
      Assert.IsType<NotFoundResult>(result);
    }
    [Fact]
    public async void GetSensorUnixTimeUtc()
    {
      //Given
      var createSensor = this.CreateSensor();
      var sensorId = createSensor.Id;
      var secretApiToken = createSensor.SecretApiToken;
      //When
      var result = await this._controllerAllowAnonymous.GetSensorUnixTimeUtc(sensorId, secretApiToken);
      var resultValue = Assert.IsType<ContentResult>(((ContentResult)result));
      //Then
      Assert.NotNull(resultValue);
    }
    [Fact]
    public async void GetSensorUnixTimeUtcNotFoundSecretApiToken()
    {
      //Given
      var createSensor = this.CreateSensor();
      var sensorId = createSensor.Id;
      var secretApiTokenInvalid = "secretApiTokenInvalid";
      //When
      var result = await this._controllerAllowAnonymous.GetSensorUnixTimeUtc(sensorId, secretApiTokenInvalid);
      //Then
      Assert.IsType<NotFoundResult>(result);
    }
  }
}