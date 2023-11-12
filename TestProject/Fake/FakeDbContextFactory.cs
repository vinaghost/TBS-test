using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace TestProject.Fake
{
    [TestClass]
    public class FakeDbContextFactory : IDbContextFactory<AppDbContext>
    {
        private static DbContextOptionsBuilder<AppDbContext> _options;

        [AssemblyInitialize]
        public static void Setup(TestContext _)
        {
            var connectionString = $"DataSource=TBS-test.db;Cache=Shared";
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .EnableSensitiveDataLogging()
                .UseSqlite(connectionString);

            using var context = new AppDbContext(_options.Options);
            Create(context);
            SeedData(context);
        }

        public AppDbContext CreateDbContext()
        {
            var context = new AppDbContext(_options.Options);
            return context;
        }

        public static void Create(AppDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        public static void SeedData(AppDbContext context)
        {
            context.Add(new Account()
            {
                Username = "test_account",
                Server = "https://thisisatestserver.com",
                Villages = new List<Village>()
                {
                    new Village()
                    {
                        Name = "test_village",
                    },
                },
            });

            context.SaveChanges();
        }
    }
}