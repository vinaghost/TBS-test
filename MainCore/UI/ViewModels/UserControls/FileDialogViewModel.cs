using MainCore.Infrasturecture.AutoRegisterDi;

namespace MainCore.UI.ViewModels.UserControls
{
    [RegisterAsSingleton(withoutInterface: true)]
    public class FileDialogViewModel
    {
        public Func<string> OpenFileDialogFunc;
        public Func<string> SaveFileDialogFunc;

        public string OpenFileDialog() => OpenFileDialogFunc?.Invoke();

        public string SaveFileDialog() => SaveFileDialogFunc?.Invoke();
    }
}