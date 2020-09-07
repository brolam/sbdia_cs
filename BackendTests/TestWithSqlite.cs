using System;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.EntityFramework.Options;
using Microsoft.Extensions.Options;

namespace BackendTest
{
    public class OperationalStoreOptionsMigrations :IOptions<OperationalStoreOptions>
    {
        public OperationalStoreOptions Value => new OperationalStoreOptions()
        {
            DeviceFlowCodes = new TableConfiguration("DeviceCodes"),
            EnableTokenCleanup = false,
            PersistedGrants = new TableConfiguration("PersistedGrants"),
            TokenCleanupBatchSize = 100,
            TokenCleanupInterval = 3600,
        };
    }
    public abstract class TestWithSqlite : IDisposable
    {
        protected readonly AppDbContext DbContext;
        protected TestWithSqlite()
        {
            var storeOptions = new OperationalStoreOptionsMigrations();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase("sbdia_db")
                    .Options;

            DbContext = new AppDbContext(options, storeOptions);
            DbContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            //AppDbContext.Dispose();
        }
    }
}
