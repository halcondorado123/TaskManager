using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Application.Service
{
    public class SweetAlertService : ISweetAlertService
    {
        private readonly IJSRuntime _js;

        public SweetAlertService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task ShowAlertAsync(string title, string message, string icon = "info")
        {
            await _js.InvokeVoidAsync("showSweetAlert", title, message, icon);
        }

        public async Task<bool> ShowConfirmAsync(string title, string message)
        {
            return await _js.InvokeAsync<bool>("showSweetConfirm", title, message);
        }

        public async Task ShowSuccessAsync(string title, string message = "")
        {
            await ShowAlertAsync(title, message, "success");
        }

        public async Task ShowErrorAsync(string title, string message = "")
        {
            await ShowAlertAsync(title, message, "error");
        }

        public async Task ShowWarningAsync(string title, string message = "")
        {
            await ShowAlertAsync(title, message, "warning");
        }

        public async Task ShowInfoAsync(string title, string message = "")
        {
            await ShowAlertAsync(title, message, "info");
        }
    }
}
