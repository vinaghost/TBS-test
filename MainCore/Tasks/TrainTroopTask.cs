using FluentResults;
using MainCore.Common.Tasks;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Repositories;

namespace MainCore.Tasks
{
    [RegisterAsTransient(withoutInterface: true)]
    public class TrainTroopTask : VillageTask
    {
        private readonly IVillageRepository _villageRepository;

        public TrainTroopTask(IVillageRepository villageRepository)
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