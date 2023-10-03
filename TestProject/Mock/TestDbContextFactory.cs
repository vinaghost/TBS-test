using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace TestProject.Mock
{
    public class TestDbContextFactory : IDbContextFactory<AppDbContext>
    {
        private const string _connectionString = "DataSource=TBS_test.db;Cache=Shared";
        private readonly DbContextOptions<AppDbContext> _options;

        public TestDbContextFactory()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connectionString)
                .Options;
        }

        public AppDbContext CreateDbContext()
        {
            return new AppDbContext(_options);
        }
    }
}