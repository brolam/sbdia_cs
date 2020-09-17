using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Backend.BackgroundServices
{
    public class SensorLogBatchBackgroundService : BackgroundService
    {
        private readonly ILogger<SensorLogBatchBackgroundService> logger;

        public SensorLogBatchBackgroundService(ILogger<SensorLogBatchBackgroundService> logger)
        {
            this.logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogDebug("SensorLogBatchBackgroundService is starting.");
            stoppingToken.Register(() => logger.LogDebug("#1 SensorLogBatchBackgroundService background task is stopping."));
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogDebug("SensorLogBatchBackgroundService background task is doing background work.");
                await Task.Delay(10000 , stoppingToken);
            }
            logger.LogDebug("SensorLogBatchBackgroundService background task is stopping.");
            await Task.CompletedTask;
        }
    }
}
