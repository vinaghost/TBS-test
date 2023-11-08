using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace TestProject.Fake
{
    public class FakeDbContextFactory : IDbContextFactory<AppDbContext>
    {
        private readonly DbContextOptionsBuilder<AppDbContext> _options;

        public FakeDbContextFactory(string testname)
        {
            var connectionString = $"DataSource=TBS-{testname}.db;Cache=Shared";
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .EnableSensitiveDataLogging()
                .UseSqlite(connectionString);
        }

        public void Setup()
        {
            using var context = new AppDbContext(_options.Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        public AppDbContext CreateDbContext()
        {
            var context = new AppDbContext(_options.Options);
            return context;
        }
    }
}