using FluentResults;
using MainCore.Commands;
using MainCore.Commands.Special;
using MainCore.Common.Enums;
using MainCore.Common.Errors;
using MainCore.Common.Errors.TrainTroop;
using MainCore.Common.Tasks;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Repositories;
using MediatR;

namespace MainCore.Tasks
{
    [RegisterAsTransient(withoutInterface: true)]
    public class TrainTroopTask : VillageTask
    {
        private readonly IUnitOfRepository _unitOfRepository;
        private readonly IUnitOfCommand _unitOfCommand;
        private readonly IMediator _mediator;

        private static readonly Dictionary<BuildingEnums, VillageSettingEnums> _settings = new()
        {
            {BuildingEnums.Barracks, VillageSettingEnums.BarrackTroop },
            {BuildingEnums.Stable, VillageSettingEnums.StableTroop },
            {BuildingEnums.Workshop, VillageSettingEnums.WorkshopTroop },
        };

        public TrainTroopTask(IUnitOfRepository unitOfRepository, IUnitOfCommand unitOfCommand, IMediator mediator)
        {
            _unitOfRepository = unitOfRepository;
            _unitOfCommand = unitOfCommand;
            _mediator = mediator;
        }

        public override async Task<Result> Execute()
        {
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            var buildings = _unitOfRepository.BuildingRepository.GetTrainTroopBuilding(VillageId);
            if (buildings.Count == 0) return Result.Ok();

            SetNextExecute();

            Result result;
            result = _unitOfCommand.SwitchVillageCommand.Execute(AccountId, VillageId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            var settings = new Dictionary<VillageSettingEnums, int>();
            foreach (var building in buildings)
            {
                result = await _mediator.Send(new TrainTroopCommand(AccountId, VillageId, building));
                if (result.IsFailed)
                {
                    if (result.HasError<MissingBuilding>())
                    {
                        settings.Add(_settings[building], 0);
                    }
                    else if (result.HasError<MissingResource>())
                    {
                        break;
                    }
                }
            }

            _unitOfRepository.VillageSettingRepository.Update(VillageId, settings);

            return Result.Ok();
        }

        public void SetNextExecute()
        {
            var minutes = _unitOfRepository.AccountSettingRepository.GetByName(AccountId, AccountSettingEnums.FarmIntervalMin, AccountSettingEnums.FarmIntervalMax);
            ExecuteAt = DateTime.Now.AddMinutes(minutes);
        }

        protected override void SetName()
        {
            var name = _unitOfRepository.VillageRepository.GetVillageName(VillageId);
            _name = $"Training troop in {name}";
        }
    }
}