using FluentResults;
using MainCore.Common.Enums;
using MainCore.Common.Errors;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Repositories;
using MainCore.Services;
using Serilog;

namespace MainCore.Commands.General
{
    [RegisterAsTransient]
    public class ChooseAccessCommand : IChooseAccessCommand
    {
        private readonly IUnitOfRepository _unitOfRepository;
        private readonly IUnitOfCommand _unitOfCommand;
        private readonly ILogService _logService;

        public ChooseAccessCommand(IUnitOfRepository unitOfRepository, IUnitOfCommand unitOfCommand, ILogService logService)
        {
            _unitOfRepository = unitOfRepository;
            _unitOfCommand = unitOfCommand;
            _logService = logService;
        }

        public async Task<Result<AccessDto>> Execute(AccountId accountId, bool ignoreSleepTime)
        {
            var logger = _logService.GetLogger(accountId);
            var accesses = _unitOfRepository.AccountRepository.GetAccesses(accountId);

            var access = await GetValidAccess(accesses, logger);
            if (access is null) return Result.Fail(NoAccessAvailable.AllAccessNotWorking);

            if (accesses.Count == 1) return access;
            if (ignoreSleepTime) return access;

            var minSleep = _unitOfRepository.AccountSettingRepository.GetByName(accountId, AccountSettingEnums.SleepTimeMin);

            var timeValid = DateTime.Now.AddMinutes(-minSleep);
            if (access.LastUsed > timeValid) return Result.Fail(NoAccessAvailable.LackOfAccess);
            return access;
        }

        private async Task<AccessDto> GetValidAccess(List<AccessDto> accesses, ILogger logger)
        {
            foreach (var access in accesses)
            {
                logger.Information("Check proxy {proxy}", access.Proxy);
                var valid = await _unitOfCommand.ValidateProxyCommand.Execute(access);
                if (!valid)
                {
                    logger.Warning("Proxy {proxy} is not working", access.Proxy);
                    continue;
                }
                return access;
            }
            return null;
        }
    }
}