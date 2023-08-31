using MainCore.Models.Database;
using ReactiveUI;

namespace WPFUI.Models.Input
{
    public class AccessInput : ReactiveObject
    {
        public AccessInput()
        {
            Password = string.Empty;
            ProxyHost = string.Empty;
            ProxyPort = 0;
            ProxyUsername = string.Empty;
            ProxyPassword = string.Empty;
        }

        public AccessInput(AccessInput other)
        {
            Password = other.Password;
            ProxyHost = other.ProxyHost;
            ProxyPort = other.ProxyPort;
            ProxyUsername = other.ProxyUsername;
            ProxyPassword = other.ProxyPassword;
        }

        public AccessInput(Access other)
        {
            Password = other.Password;
            ProxyHost = other.ProxyHost;
            ProxyPort = other.ProxyPort;
            ProxyUsername = other.ProxyUsername;
            ProxyPassword = other.ProxyPassword;
        }

        public Access GetAccess()
        {
            return new()
            {
                Password = Password,
                ProxyHost = ProxyHost,
                ProxyPort = ProxyPort,
                ProxyUsername = ProxyUsername,
                ProxyPassword = ProxyPassword
            };
        }

        public void CopyTo(AccessInput other)
        {
            other.Password = Password;
            other.ProxyHost = ProxyHost;
            other.ProxyPort = ProxyPort;
            other.ProxyUsername = ProxyUsername;
            other.ProxyPassword = ProxyPassword;
        }

        public void CopyFrom(AccessInput other)
        {
            Password = other.Password;
            ProxyHost = other.ProxyHost;
            ProxyPort = other.ProxyPort;
            ProxyUsername = other.ProxyUsername;
            ProxyPassword = other.ProxyPassword;
        }

        public void Clear()
        {
            Password = "";
            ProxyHost = "";
            ProxyPort = 0;
            ProxyUsername = "";
            ProxyPassword = "";
        }

        private string _password;
        private string _proxyHost;
        private int _proxyPort;
        private string _proxyUsername;
        private string _proxyPassword;

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
    }
}