using MainCore.Enums;
using MainCore.Models;
using MainCore.Models.Plans;
using MainCore.Repositories;
using System.Text.Json;
using TestProject.Mock;

namespace TestProject.Repositories
{
    [TestClass]
    public class JobRepositoryTest
    {
        private readonly TestDbContextFactory _contextFactory;

        public JobRepositoryTest()
        {
            _contextFactory = new TestDbContextFactory();
        }

        [TestMethod]
        public async Task GetListTest()
        {
            var jobRepository = new JobRepository(_contextFactory);

            var plan = new NormalBuildPlan()
            {
                Building = BuildingEnums.Woodcutter,
                Level = 1,
                Location = 2
            };

            await jobRepository.Add(1, plan);

            var jobs = await jobRepository.GetList(1);
            Assert.AreEqual(1, jobs.Count);

            var job = jobs[0];

            var instance = JsonSerializer.Deserialize<NormalBuildPlan>(job.Content);
            Assert.IsNotNull(instance);
            Assert.AreEqual(BuildingEnums.Woodcutter, instance.Building);
            Assert.AreEqual(1, instance.Level);
            Assert.AreEqual(2, instance.Location);
        }

        [TestInitialize]
        public void Setup()
        {
            using var context = _contextFactory.CreateDbContext();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Accounts.Add(new Account()
            {
                Id = 1,
                Server = "https://tbs.test",
                Username = "testaccount",
                Accesses = new List<Access>()
                {
                    new Access()
                    {
                        Password = "password",
                    },
                },
                Villages = new List<Village>()
                {
                    new Village()
                    {
                        Id = 1,
                        Name = "abc",
                        X = 1,
                        Y = 1,
                    }
                }
            });

            context.SaveChanges();
        }
    }
}