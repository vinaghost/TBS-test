using MainCore.Models;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace WPFUI.Models.Input
{
    public class AccessInput : ReactiveValidationObject
    {
        public AccessInput()
        {
            SetupValidationRule();
        }

        public void SetupValidationRule()
        {
            this.ValidationRule(x => x.Password,
                                x => !string.IsNullOrWhiteSpace(x),
                                "Password is empty");

            var proxyHostPortObservable = this.WhenAnyValue(x => x.ProxyHost,
                                                            x => x.ProxyPort,
                                                            (host, port) => new { Host = host, Port = port });
            this.ValidationRule(x => x.ProxyHost,
                                proxyHostPortObservable,
                                x =>
                                {
                                    if (x.Port == 0) return true;
                                    if (string.IsNullOrEmpty(x.Host)) return false;
                                    return true;
                                },
                                x => "Proxy's port is specificted but proxy's host is empty");
            this.ValidationRule(x => x.ProxyPort,
                                proxyHostPortObservable,
                                x =>
                                {
                                    if (string.IsNullOrEmpty(x.Host)) return true;
                                    if (x.Port == 0) return false;
                                    return true;
                                },
                                x => "Proxy's host is specificted but proxy's port is empty");

            var proxyUsernamePasswordObservable = this.WhenAnyValue(x => x.ProxyUsername,
                                                                    x => x.ProxyPassword,
                                                                    (username, password) => new { Username = username, Password = password });
            this.ValidationRule(x => x.ProxyUsername,
                proxyUsernamePasswordObservable,
                x =>
                {
                    if (string.IsNullOrEmpty(x.Password)) return true;
                    if (string.IsNullOrEmpty(x.Username)) return false;
                    return true;
                },
                x => "Proxy's password is specificted but proxy's username is empty");
            this.ValidationRule(x => x.ProxyPassword,
                proxyUsernamePasswordObservable,
                x =>
                {
                    if (string.IsNullOrEmpty(x.Username)) return true;
                    if (string.IsNullOrEmpty(x.Password)) return false;
                    return true;
                },
                x => "Proxy's username is specificted but proxy's password is empty");
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

        public AccessInput Clone()
        {
            return new()
            {
                _id = _id,
                Password = Password,
                ProxyHost = ProxyHost,
                ProxyPort = ProxyPort,
                ProxyUsername = ProxyUsername,
                ProxyPassword = ProxyPassword,
                Useragent = Useragent,
            };
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