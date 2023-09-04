using DynamicData;
using MainCore.Models;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;

namespace WPFUI.Models.Input
{
    public class AccountInput : ReactiveObject
    {
        public void Clear()
        {
            _id = 0;
            Username = "";
            Server = "";
            Accesses.Clear();
        }

        public Account GetAccount()
        {
            return new Account()
            {
                Id = _id,
                Username = Username,
                Server = Server,
                Accesses = Accesses.Select(x => x.GetAccess()).ToList(),
            };
        }

        public void CopyFrom(Account other)
        {
            _id = other.Id;
            Username = other.Username;
            Server = other.Server;
            var accesses = other.Accesses.Select(x => new AccessInput(x)).ToList();
            Accesses.Clear();
            Accesses.AddRange(accesses);
        }

        private int _id;
        private string _username;
        private string _server;
        public ObservableCollection<AccessInput> Accesses = new();

        public int Id => _id;

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