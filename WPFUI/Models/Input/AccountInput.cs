using DynamicData;
using MainCore.Models;
using ReactiveUI;
using System.Collections.Generic;
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

        public void CopyTo(Account other)
        {
            other.Id = _id;
            other.Username = Username;
            other.Server = Server;

            var accessAdded = new List<int>();
            foreach (var access in Accesses)
            {
                var accountAccess = other.Accesses.FirstOrDefault(x => x.Id == access.Id);
                if (accountAccess is null)
                {
                    other.Accesses.Add(access.GetAccess());
                }
                else
                {
                    access.CopyTo(accountAccess);
                }

                accessAdded.Add(access.Id);
            }
            var accessRemove = other.Accesses.Where(x => !accessAdded.Contains(x.Id)).ToList();
            foreach (var access in accessRemove)
            {
                other.Accesses.Remove(access);
            }
        }

        private int _id;
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