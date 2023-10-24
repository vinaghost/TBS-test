using MainCore.UI.ViewModels.UserControls;
using MediatR;

namespace MainCore.UI.Commands
{
    public class ShowWaitMessageCommand : IRequest
    {
        public string Message { get; }
        public Func<Task> Func { get; }

        public ShowWaitMessageCommand(string message, Func<Task> func)
        {
            Message = message;
            Func = func;
        }

        public ShowWaitMessageCommand(string message, Action func)
        {
            Message = message;
            Func = () => Task.Run(func);
        }
    }

    public class WaitMessageCommandHandler : IRequestHandler<ShowWaitMessageCommand>
    {
        private readonly WaitingOverlayViewModel _viewModel;

        public WaitMessageCommandHandler(WaitingOverlayViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public async Task Handle(ShowWaitMessageCommand request, CancellationToken cancellationToken)
        {
            await _viewModel.Show(request.Message, request.Func);
        }
    }
}