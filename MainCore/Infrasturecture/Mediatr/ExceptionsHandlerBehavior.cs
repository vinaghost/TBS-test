using MainCore.Services;
using MediatR;

namespace MainCore.Infrasturecture.Mediatr
{
    public class ExceptionsHandlerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly IDialogService _dialogService;

        public ExceptionsHandlerBehavior(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (Exception exception)
            {
                var exceptions = exception.FlattenInnerExceptions();

                foreach (var ex in exceptions)
                {
                    _dialogService.ShowMessageBox(ex.GetType().FullName, ex.Message);
                }

                throw new AggregateException("One or more errors occurred", exceptions);
            }
        }
    }
}