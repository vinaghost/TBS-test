using MainCore.CQRS.Commands;
using MainCore.DTO;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.UI.ViewModels.Abstract;
using MediatR;
using ReactiveUI;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Unit = System.Reactive.Unit;

namespace MainCore.UI.ViewModels.Tabs
{
    [RegisterAsSingleton(withoutInterface: true)]
    public class AddAccountsViewModel : TabViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly IMediator _mediator;
        public ReactiveCommand<Unit, Unit> AddAccountCommand { get; }
        public ReactiveCommand<string, Unit> UpdateTableCommand { get; }

        public ObservableCollection<AccountDto> Accounts { get; } = new();
        private string _input;

        public string Input
        {
            get => _input;
            set => this.RaiseAndSetIfChanged(ref _input, value);
        }

        public AddAccountsViewModel(IDialogService dialogService, IMediator mediator)
        {
            _mediator = mediator;
            _dialogService = dialogService;

            AddAccountCommand = ReactiveCommand.CreateFromTask(AddAccountCommandHandler);
            UpdateTableCommand = ReactiveCommand.CreateFromTask<string>(UpdateTableCommandHandler);

            this.WhenAnyValue(x => x.Input).InvokeCommand(UpdateTableCommand);
        }

        private async Task UpdateTableCommandHandler(string input)
        {
            var dtos = Parse(input);

            await Observable.Start(() =>
            {
                Accounts.Clear();
                foreach (var dto in dtos)
                {
                    if (dto is not null) continue;
                    Accounts.Add(dto);
                }
            }, RxApp.MainThreadScheduler);
        }

        private async Task AddAccountCommandHandler()
        {
            await _mediator.Send(new AddRangeAccountCommand(Accounts.ToList()));

            await Observable.Start(() =>
            {
                Accounts.Clear();
                Input = "";
            }, RxApp.MainThreadScheduler);

            _dialogService.ShowMessageBox("Information", "Added accounts");
        }

        private static List<AccountDto> Parse(string input)
        {
            if (string.IsNullOrEmpty(input)) return new List<AccountDto>();
            var strArr = input.Trim().Split('\n');
            var accounts = new ConcurrentBag<AccountDto>();
            Parallel.ForEach(strArr, str => accounts.Add(ParseLine(str)));
            return accounts.ToList();
        }

        private static AccountDto ParseLine(string input)
        {
            var strAccount = input.Trim().Split(' ');
            Uri url = null;
            if (strAccount.Length > 0)
            {
                if (!Uri.TryCreate(strAccount[0], UriKind.Absolute, out url))
                {
                    return null;
                };
            }

            if (strAccount.Length > 4)
            {
                if (int.TryParse(strAccount[4], out var port))
                {
                    strAccount[4] = port.ToString();
                }
                else
                {
                    return null;
                }
            }
            return strAccount.Length switch
            {
                3 => AccountDto.Create(url.AbsoluteUri, strAccount[1], strAccount[2]),
                5 => AccountDto.Create(url.AbsoluteUri, strAccount[1], strAccount[2], strAccount[3], int.Parse(strAccount[4])),
                7 => AccountDto.Create(url.AbsoluteUri, strAccount[1], strAccount[2], strAccount[3], int.Parse(strAccount[4]), strAccount[5], strAccount[6]),
                _ => null,
            }; ;
        }
    }
}