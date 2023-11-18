using FluentResults;
using MainCore.Commands.Base;
using MainCore.Common.Enums;
using MainCore.Common.Errors;
using MainCore.Entities;
using MainCore.Repositories;
using MediatR;

namespace MainCore.Commands.Special
{
    public class ToNPCPageCommand : ByAccountVillageIdRequestBase, IRequest<Result>
    {
        public ToNPCPageCommand(AccountId accountId, VillageId villageId) : base(accountId, villageId)
        {
        }
    }

    public class ToNPCPageCommandHandler : IRequestHandler<ToNPCPageCommand, Result>
    {
        private readonly IUnitOfRepository _unitOfRepository;
        private readonly IUnitOfCommand _unitOfCommand;

        public ToNPCPageCommandHandler(IUnitOfRepository unitOfRepository, IUnitOfCommand unitOfCommand)
        {
            _unitOfRepository = unitOfRepository;
            _unitOfCommand = unitOfCommand;
        }

        public async Task<Result> Handle(ToNPCPageCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;
            var villageId = request.VillageId;
            Result result;
            result = await _unitOfCommand.UpdateVillageListCommand.Execute(accountId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = _unitOfCommand.SwitchVillageCommand.Execute(accountId, villageId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = _unitOfCommand.ToDorfCommand.Execute(accountId, 2);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = await _unitOfCommand.UpdateDorfCommand.Execute(accountId, villageId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            var market = _unitOfRepository.BuildingRepository.GetBuildingLocation(villageId, BuildingEnums.Marketplace);

            result = _unitOfCommand.ToBuildingCommand.Execute(accountId, market);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = _unitOfCommand.SwitchTabCommand.Execute(accountId, 1);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }
    }
}