using MainCore.Common.Repositories;
using MainCore.DTO;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.UI.ViewModels.Abstract;
using ReactiveUI;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;

namespace MainCore.UI.ViewModels.Tabs
{
    [RegisterAsSingleton(withoutInterface: true)]
    public class AddAccountsViewModel : TabViewModelBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IDialogService _dialogService;
        public ReactiveCommand<Unit, Unit> AddAccountCommand { get; }
        public ReactiveCommand<string, Unit> UpdateTableCommand { get; }

        public ObservableCollection<AccountDto> Accounts { get; } = new();
        private string _input;

        public string Input
        {
            get => _input;
            set => this.RaiseAndSetIfChanged(ref _input, value);
        }

        public AddAccountsViewModel(IAccountRepository accountRepository, IDialogService dialogService)
        {
            _accountRepository = accountRepository;

            AddAccountCommand = ReactiveCommand.CreateFromTask(AddAccountTask);
            UpdateTableCommand = ReactiveCommand.CreateFromTask<string>(UpdateTableTask);

            this.WhenAnyValue(x => x.Input).InvokeCommand(UpdateTableCommand);
            _dialogService = dialogService;
        }

        private async Task UpdateTableTask(string input)
        {
            await Observable.Start(
                () => Accounts.Clear(),
                RxApp.MainThreadScheduler);

            if (string.IsNullOrWhiteSpace(input))
            {
                return;
            }

            var dtos = await Observable.Start(() =>
            {
                var strArr = input.Trim().Split('\n');
                ConcurrentBag<AccountDto> accounts = new();
                Parallel.ForEach(strArr, str => accounts.Add(Parse(str)));
                return accounts.ToList();
            }, RxApp.TaskpoolScheduler);

            await Observable.Start(() =>
            {
                foreach (var dto in dtos)
                {
                    if (dto is not null) continue;
                    Accounts.Add(dto);
                }
            }, RxApp.MainThreadScheduler);
        }

        private async Task AddAccountTask()
        {
            await Observable.StartAsync(
                () => _accountRepository.AddRange(Accounts.ToList()),
                RxApp.TaskpoolScheduler);

            await Observable.Start(() =>
            {
                Accounts.Clear();
                Input = "";
            }, RxApp.MainThreadScheduler);

            _dialogService.ShowMessageBox("Information", "Added accounts");
        }

        private static AccountDto Parse(string input)
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