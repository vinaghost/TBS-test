using DynamicData;
using MainCore.Common.Repositories;
using MainCore.DTO;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.UI.ViewModels.Abstract;
using MainCore.UI.ViewModels.UserControls;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;

namespace MainCore.UI.ViewModels.Tabs
{
    [RegisterAsSingleton(withoutInterface: true)]
    public class AddAccountsViewModel : TabViewModelBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;
        public ReactiveCommand<Unit, Unit> AddAccountCommand { get; }
        public ReactiveCommand<string, Unit> UpdateTableCommand { get; }

        public ObservableCollection<AccountsDto> Accounts { get; } = new();
        private string _input;

        public string Input
        {
            get => _input;
            set => this.RaiseAndSetIfChanged(ref _input, value);
        }

        public AddAccountsViewModel(IAccountRepository accountRepository, WaitingOverlayViewModel waitingOverlayViewModel)
        {
            _accountRepository = accountRepository;
            _waitingOverlayViewModel = waitingOverlayViewModel;

            AddAccountCommand = ReactiveCommand.CreateFromTask(AddAccountTask);
            UpdateTableCommand = ReactiveCommand.CreateFromTask<string>(UpdateTableTask);

            this.WhenAnyValue(x => x.Input).InvokeCommand(UpdateTableCommand);
        }

        private async Task UpdateTableTask(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                if (Accounts.Count > 0) Accounts.Clear();
                return;
            }
            var strArr = input.Trim().Split('\n');
            var listTasks = new List<Task<AccountsDto>>();
            foreach (var str in strArr)
            {
                listTasks.Add(Task.Run(() => AccountParser(str)));
            }

            var listResult = await Task.WhenAll(listTasks);
            listResult = listResult.Where(x => x is not null).ToArray();

            Accounts.Clear();
            Accounts.AddRange(listResult);
        }

        private async Task AddAccountTask()
        {
            _waitingOverlayViewModel.Show("adding accounts ...");
            await Observable.StartAsync(() => _accountRepository.AddRange(Accounts.ToList()), RxApp.TaskpoolScheduler);
            Accounts.Clear();
            Input = "";
            _waitingOverlayViewModel.Close();
        }

        private static AccountsDto AccountParser(string input)
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
                3 => new AccountsDto()
                {
                    Server = url.AbsoluteUri,
                    Username = strAccount[1],
                    Password = strAccount[2],
                },
                5 => new AccountsDto()
                {
                    Server = url.AbsoluteUri,
                    Username = strAccount[1],
                    Password = strAccount[2],
                    ProxyHost = strAccount[3],
                    ProxyPort = int.Parse(strAccount[4]),
                },
                7 => new AccountsDto()
                {
                    Server = url.AbsoluteUri,
                    Username = strAccount[1],
                    Password = strAccount[2],
                    ProxyHost = strAccount[3],
                    ProxyPort = int.Parse(strAccount[4]),
                    ProxyUsername = strAccount[5],
                    ProxyPassword = strAccount[6],
                },
                _ => null,
            };
        }
    }
}