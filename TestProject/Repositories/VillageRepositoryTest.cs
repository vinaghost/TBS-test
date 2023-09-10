﻿using MainCore.Models;
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
            var villages = await villageRepository.Get(1);
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
            var villages = await villageRepository.Get(1);
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
            var villages = await villageRepository.Get(1);
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