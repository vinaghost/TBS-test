using FluentResults;
using MainCore.Common.Tasks;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Repositories;

namespace MainCore.Features.TroopTraining.Tasks
{
    [RegisterAsTransient(withoutInterface: true)]
    public class TrainingTroopTask : VillageTask
    {
        private readonly IVillageRepository _villageRepository;

        public TrainingTroopTask(IVillageRepository villageRepository)
        {
            _villageRepository = villageRepository;
        }

        public override Task<Result> Execute()
        {
            throw new NotImplementedException();
        }

        protected override void SetName()
        {
            var name = _villageRepository.GetVillageName(VillageId);
            _name = $"Training troop in {name}";
        }
    }
}