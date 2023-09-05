using MainCore.Models;
using ReactiveUI;

namespace WPFUI.Models.Input
{
    public class AccessInput : ReactiveObject
    {
        public AccessInput()
        {
            _id = 0;
            Password = "";
            ProxyHost = "";
            ProxyPort = 0;
            ProxyUsername = "";
            ProxyPassword = "";
            Useragent = "";
        }

        public AccessInput(AccessInput other)
        {
            _id = other._id;
            Password = other.Password;
            ProxyHost = other.ProxyHost;
            ProxyPort = other.ProxyPort;
            ProxyUsername = other.ProxyUsername;
            ProxyPassword = other.ProxyPassword;
            Useragent = other.Useragent;
        }

        public AccessInput(Access other)
        {
            _id = other.Id;
            Password = other.Password;
            ProxyHost = other.ProxyHost;
            ProxyPort = other.ProxyPort;
            ProxyUsername = other.ProxyUsername;
            ProxyPassword = other.ProxyPassword;
            Useragent = other.Useragent;
        }

        public Access GetAccess()
        {
            return new()
            {
                Id = _id,
                Password = Password,
                ProxyHost = ProxyHost,
                ProxyPort = ProxyPort,
                ProxyUsername = ProxyUsername,
                ProxyPassword = ProxyPassword,
                Useragent = Useragent,
            };
        }

        public void CopyTo(AccessInput other)
        {
            other._id = _id;
            other.Password = Password;
            other.ProxyHost = ProxyHost;
            other.ProxyPort = ProxyPort;
            other.ProxyUsername = ProxyUsername;
            other.ProxyPassword = ProxyPassword;
            other.Useragent = Useragent;
        }

        public void CopyTo(Access other)
        {
            other.Password = Password;
            other.ProxyHost = ProxyHost;
            other.ProxyPort = ProxyPort;
            other.ProxyUsername = ProxyUsername;
            other.ProxyPassword = ProxyPassword;
            other.Useragent = Useragent;
        }

        public void CopyFrom(AccessInput other)
        {
            _id = other._id;
            Password = other.Password;
            ProxyHost = other.ProxyHost;
            ProxyPort = other.ProxyPort;
            ProxyUsername = other.ProxyUsername;
            ProxyPassword = other.ProxyPassword;
            Useragent = other.Useragent;
        }

        public void Clear()
        {
            _id = 0;
            Password = "";
            ProxyHost = "";
            ProxyPort = 0;
            ProxyUsername = "";
            ProxyPassword = "";
            Useragent = "";
        }

        private int _id;
        private string _password;
        private string _proxyHost;
        private int _proxyPort;
        private string _proxyUsername;
        private string _proxyPassword;
        private string _useragent;

        public int Id => _id;

        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public string ProxyHost
        {
            get => _proxyHost;
            set => this.RaiseAndSetIfChanged(ref _proxyHost, value);
        }

        public int ProxyPort
        {
            get => _proxyPort;
            set => this.RaiseAndSetIfChanged(ref _proxyPort, value);
        }

        public string ProxyUsername
        {
            get => _proxyUsername;
            set => this.RaiseAndSetIfChanged(ref _proxyUsername, value);
        }

        public string ProxyPassword
        {
            get => _proxyPassword;
            set => this.RaiseAndSetIfChanged(ref _proxyPassword, value);
        }

        public string Useragent
        {
            get => _useragent;
            set => this.RaiseAndSetIfChanged(ref _useragent, value);
        }
    }
}