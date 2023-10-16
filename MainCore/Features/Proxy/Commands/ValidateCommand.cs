using FluentResults;
using MainCore.DTO;
using MainCore.Features.Proxy.Errors;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using Polly;
using Polly.Retry;
using RestSharp;

namespace MainCore.Features.Proxy.Commands
{
    [RegisterAsTransient]
    public class ValidateCommand : IValidateCommand
    {
        private readonly IRestClientManager _restClientManager;

        public ValidateCommand(IRestClientManager restClientManager)
        {
            _restClientManager = restClientManager;
        }

        private readonly AsyncRetryPolicy<bool> _retryPolicy = Policy<bool>
                .Handle<Exception>()
                .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider: _ => TimeSpan.FromSeconds(10));

        public async Task<Result> Execute(AccessDto access)
        {
            var poliResult = await _retryPolicy
                .ExecuteAndCaptureAsync(() => Validate(access));

            if (!poliResult.Result) return Result.Fail(new ProxyNotWork(access));
            return Result.Ok();
        }

        private async Task<bool> Validate(AccessDto access)
        {
            var request = new RestRequest
            {
                Method = Method.Get,
            };
            var client = _restClientManager.Get(access);
            var response = await client.ExecuteAsync(request);
            return !string.IsNullOrWhiteSpace(response.Content);
        }
    }
}