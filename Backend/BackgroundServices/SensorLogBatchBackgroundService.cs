using System.Threading;
using System.Threading.Tasks;
using Backend.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Backend.BackgroundServices
{
    public class SensorLogBatchBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<SensorLogBatchBackgroundService> logger;

        public SensorLogBatchBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<SensorLogBatchBackgroundService> logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogDebug("SensorLogBatchBackgroundService is starting.");
            stoppingToken.Register(() => logger.LogDebug("#1 SensorLogBatchBackgroundService background task is stopping."));
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogDebug("SensorLogBatchBackgroundService background task is doing background work.");
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var sensorWithLogBatchPending = await dbContext.GetSensorWithLogBatchPending();
                    foreach (var sensor in sensorWithLogBatchPending)
                    {
                        dbContext.PerformContentSensorLogBatch(sensor);
                        logger.LogDebug($"Sensor {sensor.Name}  background task");
                    }
                }
                await Task.Delay(10000, stoppingToken);
            }
            logger.LogDebug("SensorLogBatchBackgroundService background task is stopping.");
            await Task.CompletedTask;
        }
    }
}
