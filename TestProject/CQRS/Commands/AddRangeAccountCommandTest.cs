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
    public class AddRangeAccountCommandTest
    {
        [TestMethod]
        public void ShouldNotThrow()
        {
            var handler = Setup(nameof(ShouldNotThrow));
            var dtos = new List<AccountDetailDto>() {
                new AccountDetailDto()
                {
                    Username = "test",
                    Server = "https://test.com/",
                    Password="passtest",
                }
            };

            var func = () => handler.Add(dtos);
            func.Should().NotThrow();
        }

        public static AddRangeAccountCommandHandler Setup(string testname)
        {
            var mediator = A.Fake<IMediator>();
            var useragentManager = A.Fake<IUseragentManager>();
            A.CallTo(() => useragentManager.Get()).Returns("thisisafakeuseragent");
            var dbContextFactory = new FakeDbContextFactory(testname);
            dbContextFactory.Setup();
            var handler = new AddRangeAccountCommandHandler(dbContextFactory, mediator, useragentManager);
            return handler;
        }
    }
}