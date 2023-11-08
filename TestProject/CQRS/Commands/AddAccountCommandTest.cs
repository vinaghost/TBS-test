using FakeItEasy;
using FluentAssertions;
using MainCore.CQRS.Commands;
using MainCore.DTO;
using MainCore.Infrasturecture.Services;
using MediatR;
using TestProject.Fake;

namespace TestProject.CQRS.Commands
{
    [TestClass]
    public class AddAccountCommandTest
    {
        [TestMethod]
        public void ShouldTrueWhenAccountNotDuplicate()
        {
            var handler = Setup(nameof(ShouldTrueWhenAccountNotDuplicate));
            var dto = new AccountDto()
            {
                Username = "test",
                Server = "https://test.com/",
                Accesses = new()
                {
                    new()
                    {
                        Password="passtest",
                    }
                }
            };

            handler.Add(dto).Should().BeTrue();
        }

        [TestMethod]
        public void ShouldFalseWhenAccountDuplicate()
        {
            var handler = Setup(nameof(ShouldFalseWhenAccountDuplicate));
            var dto = new AccountDto()
            {
                Username = "test",
                Server = "https://test.com/",
                Accesses = new()
                {
                    new()
                    {
                        Password="passtest",
                    }
                }
            };

            handler.Add(dto);
            handler.Add(dto).Should().BeFalse();
        }

        public static AddAccountCommandHandler Setup(string testname)
        {
            var mediator = A.Fake<IMediator>();
            var useragentManager = A.Fake<IUseragentManager>();
            A.CallTo(() => useragentManager.Get()).Returns("thisisafakeuseragent");
            var dbContextFactory = new FakeDbContextFactory(testname);
            dbContextFactory.Setup();
            var handler = new AddAccountCommandHandler(dbContextFactory, mediator, useragentManager);
            return handler;
        }
    }
}