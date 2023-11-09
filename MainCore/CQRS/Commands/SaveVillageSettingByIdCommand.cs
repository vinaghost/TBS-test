using MainCore.Common.Enums;
using MainCore.CQRS.Base;
using MainCore.Entities;
using MainCore.Repositories;
using MediatR;

namespace MainCore.CQRS.Commands
{
    public class SaveVillageSettingByIdCommand : ByVillageIdRequestBase, IRequest
    {
        public Dictionary<VillageSettingEnums, int> Settings { get; }

        public SaveVillageSettingByIdCommand(VillageId VillageId, Dictionary<VillageSettingEnums, int> settings) : base(VillageId)
        {
            Settings = settings;
        }
    }

    public class SaveVillageSettingByIdCommandHandler : IRequestHandler<SaveVillageSettingByIdCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SaveVillageSettingByIdCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(SaveVillageSettingByIdCommand request, CancellationToken cancellationToken)
        {
            await Task.Run(() => _unitOfWork.VillageSettingRepository.Update(request.VillageId, request.Settings), cancellationToken);
        }
    }
}