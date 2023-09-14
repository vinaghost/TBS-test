using MainCore.Enums;
using MainCore.Models;
using MainCore.Repositories;
using TestProject.Mock;

namespace TestProject.Repositories
{
    [TestClass]
    public class VillageRepositoryTest
    {
        private readonly TestDbContextFactory _contextFactory;

        public VillageRepositoryTest()
        {
            _contextFactory = new TestDbContextFactory();
        }

        [TestMethod]
        public async Task GetUnloadListTest()
        {
            var villageRepository = new VillageRepository(_contextFactory);
            var buildingRepository = new BuildingRepository(_contextFactory);
            var sampleVillages = new List<Village>()
            {
                new Village(){Id = 1, Name = "Abc", AccountId = 1},
                new Village(){Id = 2, Name = "xyz", AccountId = 1},
                new Village(){Id = 3, Name = "asd", AccountId = 1},
                new Village(){Id = 4, Name = "qwe", AccountId = 1},
            };
            var sampleBuildings = new List<Building>();
            for (var i = 0; i < 37; i++)
            {
                sampleBuildings.Add(new Building()
                {
                    VillageId = 1,
                    Location = i + 1,
                    IsUnderConstruction = false,
                    Level = 1,
                    Type = BuildingEnums.Site,
                });
            }
            await villageRepository.Update(1, sampleVillages);
            await buildingRepository.Update(1, sampleBuildings);
            var villages = await villageRepository.GetUnloadList(1);
            Assert.AreEqual(3, villages.Count);
        }

        [TestMethod]
        public async Task UpdateTest_AddOnly()
        {
            var villageRepository = new VillageRepository(_contextFactory);
            var sampleVillages = new List<Village>()
            {
                new Village(){Id = 1, Name = "Abc", AccountId = 1},
                new Village(){Id = 2, Name = "xyz", AccountId = 1},
                new Village(){Id = 3, Name = "asd", AccountId = 1},
                new Village(){Id = 4, Name = "qwe", AccountId = 1},
            };
            await villageRepository.Update(1, sampleVillages);
            var villages = await villageRepository.GetList(1);
            Assert.AreEqual(4, villages.Count);
        }

        [TestMethod]
        public async Task UpdateTest_DeleteOnly()
        {
            var villageRepository = new VillageRepository(_contextFactory);
            var sampleVillages = new List<Village>()
            {
                new Village(){Id = 1, Name = "Abc", AccountId = 1},
                new Village(){Id = 2, Name = "xyz", AccountId = 1},
                new Village(){Id = 3, Name = "asd", AccountId = 1},
                new Village(){Id = 4, Name = "qwe", AccountId = 1},
            };
            await villageRepository.Update(1, sampleVillages);

            sampleVillages = new List<Village>()
            {
                new Village(){Id = 1, Name = "Abc", AccountId = 1},
            };
            await villageRepository.Update(1, sampleVillages);
            var villages = await villageRepository.GetList(1);
            Assert.AreEqual(1, villages.Count);
        }

        [TestMethod]
        public async Task UpdateTest_AddDelete()
        {
            var villageRepository = new VillageRepository(_contextFactory);
            var sampleVillages = new List<Village>()
            {
                new Village(){Id = 1, Name = "Abc", AccountId = 1},
                new Village(){Id = 2, Name = "xyz", AccountId = 1},
                new Village(){Id = 3, Name = "asd", AccountId = 1},
                new Village(){Id = 4, Name = "qwe", AccountId = 1},
            };
            await villageRepository.Update(1, sampleVillages);

            sampleVillages = new List<Village>()
            {
                new Village(){Id = 2, Name = "xyz", AccountId = 1},
                new Village(){Id = 3, Name = "asd", AccountId = 1},
                new Village(){Id = 4, Name = "qwe", AccountId = 1},
                new Village(){Id = 5, Name = "qwer", AccountId = 1},
            };
            await villageRepository.Update(1, sampleVillages);
            var villages = await villageRepository.GetList(1);
            Assert.AreEqual(4, villages.Count);
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
            });

            context.SaveChanges();
        }
    }
}