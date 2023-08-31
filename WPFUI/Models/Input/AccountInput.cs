using DynamicData;
using MainCore.Models.Database;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;

namespace WPFUI.Models.Input
{
    public class AccountInput : ReactiveObject
    {
        public void Clear()
        {
            Username = "";
            Server = "";
            Accesses.Clear();
        }

        public Account GetAccount()
        {
            return new Account()
            {
                Username = Username,
                Server = Server,
                Accesses = Accesses.Select(x => x.GetAccess()).ToList(),
            };
        }

        public void CopyFrom(Account other)
        {
            Username = other.Username;
            Server = other.Server;
            var accesses = other.Accesses.Select(x => new AccessInput(x)).ToList();
            Accesses.Clear();
            Accesses.AddRange(accesses);
        }

        private string _username;
        private string _server;
        public ObservableCollection<AccessInput> Accesses = new();

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
}