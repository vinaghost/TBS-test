using FluentResults;
using MainCore.DTO;
using MainCore.Features.Update.Parsers;
using MainCore.Features.Update.Trigger;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using MediatR;

namespace MainCore.Features.Update.Commands
{
    public class UpdateAccountInfoCommand : IRequest<Result>
    {
        public int AccountId { get; }

        public UpdateAccountInfoCommand(int accountId)
        {
            AccountId = accountId;
        }
    }

    public class UpdateAccountInfoCommandHandler : IRequestHandler<UpdateAccountInfoCommand, Result>
    {
        private readonly IChromeManager _chromeManager;
        private readonly IAccountInfoParser _accountInfoParser;
        private readonly AppDbContext _context;
        private readonly IMediator _mediator;

        public UpdateAccountInfoCommandHandler(IChromeManager chromeManager, IAccountInfoParser accountInfoParser, AppDbContext context, IMediator mediator)
        {
            _chromeManager = chromeManager;
            _accountInfoParser = accountInfoParser;
            _context = context;
            _mediator = mediator;
        }

        public async Task<Result> Handle(UpdateAccountInfoCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var dto = _accountInfoParser.Get(html);
            await Task.Run(() => Update(accountId, dto), cancellationToken);
            await _mediator.Publish(new AccountInfoTrigger(accountId), cancellationToken);
            return Result.Ok();
        }

        private void Update(int accountId, AccountInfoDto dto)
        {
            var dbAccountInfo = _context.AccountsInfo
                .FirstOrDefault(x => x.AccountId == accountId);

            var mapper = new AccountInfoMapper();
            if (dbAccountInfo is null)
            {
                var accountInfo = mapper.Map(accountId, dto);
                _context.Add(accountInfo);
            }
            else
            {
                mapper.MapToEntity(dto, dbAccountInfo);
                _context.Update(dbAccountInfo);
            }
            _context.SaveChanges();
        }
    }
}