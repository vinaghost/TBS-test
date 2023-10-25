using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using System.Collections.Concurrent;
using System.Reflection;

namespace MainCore.Infrasturecture.Services
{
    [RegisterAsSingleton]
    public sealed class ChromeManager : IChromeManager
    {
        private readonly ConcurrentDictionary<AccountId, ChromeBrowser> _dictionary = new();
        private string[] _extensionsPath;
        private readonly AppDbContext _context;

        public ChromeManager(AppDbContext context)
        {
            _context = context;
        }

        public IChromeBrowser Get(AccountId accountId)
        {
            var result = _dictionary.TryGetValue(accountId, out ChromeBrowser browser);
            if (result) return browser;

            var account = _context.Accounts.Find(accountId);

            browser = new ChromeBrowser(_extensionsPath);
            _dictionary.TryAdd(accountId, browser);
            return browser;
        }

        public void Shutdown()
        {
            foreach (var id in _dictionary.Keys)
            {
                _dictionary.Remove(id, out ChromeBrowser browser);
                browser.Shutdown();
            }
        }

        public void LoadExtension()
        {
            var extenstionDir = Path.Combine(AppContext.BaseDirectory, "ExtensionFile");
            var isCreated = false;
            if (Directory.Exists(extenstionDir)) isCreated = true;
            else Directory.CreateDirectory(extenstionDir);

            var asmb = Assembly.GetExecutingAssembly();
            var extensionsName = asmb.GetManifestResourceNames();
            var list = new List<string>();

            foreach (var extensionName in extensionsName)
            {
                if (!extensionName.Contains(".crx")) continue;
                var path = Path.Combine(extenstionDir, extensionName);
                list.Add(path);
                if (!isCreated)
                {
                    using Stream input = asmb.GetManifestResourceStream(extensionName);
                    using Stream output = File.Create(path);
                    input.CopyTo(output);
                }
            }

            _extensionsPath = list.ToArray();
        }
    }
}