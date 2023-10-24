namespace MainCore.Infrasturecture.Services
{
    public interface IDialogService
    {
        string OpenFileDialog();
        string SaveFileDialog();
        bool ShowConfirmBox(string message, string title);
        void ShowMessageBox(string message, string title);
    }
}