using System.Threading;
using System.Threading.Tasks;
using Backend.BackgroundServices;
using Backend.Data;
using Backend.Models;
using Backend.Models.Dtos;
using BackendTests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace BackendTests
{
  public class SensorLogBatchBackgroundServiceTests : AppDbContextMock
  {

    [Fact]
    public async Task ExecuteAsyncTestAsync()
    {
      //Given
      var sensor_01 = CreateSensorEnergyLog("Sensor 01");
      var sensor_02 = CreateSensorEnergyLog("Sensor 02");
      await this.DbContext.CreateSensorLogBatch(sensor_01, "1574608800;1;2;3");
      await this.DbContext.CreateSensorLogBatch(sensor_02, "1574608800;1;2;3");
      var sensorWithLogBatchPending = await this.DbContext.GetSensorWithLogBatchPending();
      var recentEneryLogsSensor_01 = await this.DbContext.GetSensorEnergyLogsRecentAsync(sensor_01);
      var recentEneryLogsSensor_02 = await this.DbContext.GetSensorEnergyLogsRecentAsync(sensor_02);

      //Then
      Assert.NotEmpty(sensorWithLogBatchPending);
      Assert.Empty(recentEneryLogsSensor_01);
      Assert.Empty(recentEneryLogsSensor_02);

      //When
      this.runSensorLogBatchBackgroundService();
      sensorWithLogBatchPending = await this.DbContext.GetSensorWithLogBatchPending();
      recentEneryLogsSensor_01 = await this.DbContext.GetSensorEnergyLogsRecentAsync(sensor_01);
      recentEneryLogsSensor_02 = await this.DbContext.GetSensorEnergyLogsRecentAsync(sensor_02);

      //Then
      Assert.Empty(sensorWithLogBatchPending);
      Assert.NotEmpty(recentEneryLogsSensor_01);
      Assert.NotEmpty(recentEneryLogsSensor_02);
    }
    private async void runSensorLogBatchBackgroundService()
    {
      ServiceProvider serviceProvider;
      serviceProvider = new ServiceCollection()
     .AddLogging()
     .AddSingleton<AppDbContext>(dbContext => this.DbContext)
     .AddHostedService<SensorLogBatchBackgroundService>()
     .BuildServiceProvider();
      var hostedService = serviceProvider.GetService<IHostedService>();
      await hostedService.StartAsync(CancellationToken.None);
      await Task.Delay(3000);
      await hostedService.StopAsync(CancellationToken.None);
    }

    private Sensor CreateSensorEnergyLog(string sensorName)
    {
      var newUser = new Owner() { Email = "user@email.com" };
      this.DbContext.Add(newUser);
      this.DbContext.SaveChanges();
      var sensor = this.DbContext.CreateSensor(newUser.Id, new SensorItemDto()
      {
        Name = sensorName,
        SensorType = SensorTypes.EnergyLog,
        TimeZone = System.TimeZoneInfo.Local.Id
      });
      return sensor;
    }
  }
}
