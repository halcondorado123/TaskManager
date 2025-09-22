namespace TaskManager.Application.Service
{
    public interface ISweetAlertService
    {
        Task ShowAlertAsync(string title, string message, string icon = "info");
        Task<bool> ShowConfirmAsync(string title, string message);
        Task ShowSuccessAsync(string title, string message = "");
        Task ShowErrorAsync(string title, string message = "");
        Task ShowWarningAsync(string title, string message = "");
        Task ShowInfoAsync(string title, string message = "");
    }
}
