using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.Infrasturecture.Services
{
    [RegisterAsSingleton]
    public class DialogService : IDialogService
    {
        public Action<string, string> MessageBoxFunc;
        public Func<string, string, bool> ConfirmBoxFunc;
        public Func<string> OpenFileDialogFunc;
        public Func<string> SaveFileDialogFunc;

        public string OpenFileDialog() => OpenFileDialogFunc?.Invoke();

        public string SaveFileDialog() => SaveFileDialogFunc?.Invoke();

        public void ShowMessageBox(string message, string title) => MessageBoxFunc?.Invoke(message, title);

        public bool ShowConfirmBox(string message, string title) => ConfirmBoxFunc?.Invoke(message, title) ?? false;
    }
}