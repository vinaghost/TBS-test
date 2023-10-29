using MainCore.DTO;
using ReactiveUI;
using Riok.Mapperly.Abstractions;

namespace MainCore.UI.Models.Input
{
    public class AccessInput : ReactiveObject
    {
        public void Clear()
        {
            Password = "";
            ProxyHost = "";
            ProxyPort = 0;
            ProxyUsername = "";
            ProxyPassword = "";
            Useragent = "";
        }

        public int Id { get; set; }
        private string _password;
        private string _proxyHost;
        private int _proxyPort;
        private string _proxyUsername;
        private string _proxyPassword;
        private string _useragent;

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

    [Mapper]
    public partial class AccessInputMapper
    {
        public partial AccessDto Map(AccessInput input);

        public partial void Map(AccessDto dto, AccessInput input);
    }
}