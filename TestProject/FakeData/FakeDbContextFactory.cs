using MainCore.Infrasturecture.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TestProject.FakeData
{
    public class FakeDbContextFactory : IDbContextFactory<AppDbContext>
    {
        private readonly string _connectionString;

        public FakeDbContextFactory(string name)
        {
            _connectionString = $"DataSource=TBS-{name}.db;Cache=Shared";
        }

        public AppDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector>()
                .UseSqlite(_connectionString);

            var context = new AppDbContext(optionsBuilder.Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }
    }
}