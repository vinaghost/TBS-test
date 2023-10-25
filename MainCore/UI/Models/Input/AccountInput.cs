using DynamicData;
using MainCore.DTO;
using MainCore.Entities;
using ReactiveUI;
using Riok.Mapperly.Abstractions;
using System.Collections.ObjectModel;

namespace MainCore.UI.Models.Input
{
    public class AccountInput : ReactiveObject
    {
        public AccountId Id { get; set; }
        private string _username;
        private string _server;
        public ObservableCollection<AccessDto> Accesses = new();

        public void SetAccesses(ICollection<AccessDto> accesses)
        {
            Accesses.Clear();
            Accesses.AddRange(accesses);
        }

        public string Username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        public string Server
        {
            get => _server;
            set => this.RaiseAndSetIfChanged(ref _server, value);
        }
    }

    [Mapper]
    public partial class AccountInputMapper
    {
        public partial AccountDto Map(AccountInput input);

        public void Map(AccountDto account, AccountInput input)
        {
            MapInput(account, input);
            input.Accesses.Clear();
            input.Accesses.AddRange(account.Accesses);
        }

        [MapperIgnoreTarget(nameof(AccountInput.Accesses))]
        private partial void MapInput(AccountDto account, AccountInput input);
    }
}