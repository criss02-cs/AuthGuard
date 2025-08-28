using CommunityToolkit.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otp.Maui.Utils
{
    public class MauiAlert
    {
        public static async Task Alert(string message)
        {
            var page = Application.Current?.Windows[0].Page;
            if (page is null) return;
            await MainThread.InvokeOnMainThreadAsync(() => page.DisplayAlert("GBsoftware", message, "Ok"));
        }

        public static async Task<string> ShowItemsDialog(string title, List<string> items)
        {
            var page = Application.Current?.Windows[0].Page;
            if (page is null) return "";
            var item = await MainThread.InvokeOnMainThreadAsync(() =>
                page.DisplayActionSheet(title, "Annulla", "", [.. items]));
            return item ?? "";
        }

        public static async Task Toast(string text, ToastDuration duration)
        {
            var cancellationSource = new CancellationTokenSource();
            var toast = CommunityToolkit.Maui.Alerts.Toast.Make(text, duration);
            await toast.Show(cancellationSource.Token);
        }

        public static async Task<string> ShowInputDialog(string title, string message, Keyboard? keyboard = null, int maxLength = 9999)
        {
            var page = Application.Current?.Windows[0].Page;
            if (page is null) return string.Empty;
            keyboard ??= Keyboard.Default;
            var item = await MainThread.InvokeOnMainThreadAsync(() =>
                page.DisplayPromptAsync(title, message, "Ok", "Annulla", maxLength: maxLength, keyboard: keyboard));
            return item;
        }

        public static async Task<bool> ShowConfirmDialog(string title, string message)
        {
            var page = Application.Current?.Windows[0].Page;
            if (page is null) return false;
            var res = await MainThread.InvokeOnMainThreadAsync(() => page.DisplayAlert(title, message, "Sì", "No"));
            return res;
        }
    }

    public record AlertButton(string Text, Action OnPress);
}