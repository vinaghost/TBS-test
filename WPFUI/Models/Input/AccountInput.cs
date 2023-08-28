using ReactiveUI;
using System.Collections.ObjectModel;

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