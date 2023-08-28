namespace WPFUI.Services
{
    public interface IMessageService
    {
        bool Show(string title, string message);
        bool ShowYesNo(string title, string message);
    }
}