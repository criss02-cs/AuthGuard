using Otp.Business.Dto;
using Otp.Business.Models;
using OtpNet;

namespace Otp.Maui.Views;

public partial class UpdateOtpView : ContentPage
{
    private readonly TaskCompletionSource<OtpAccount?> _pageResultCompletionSource;

    public UpdateOtpView(OtpAccount? otp = null)
    {
        InitializeComponent();
        dataForm.DataObject = otp ?? new OtpAccount();
        _pageResultCompletionSource = new TaskCompletionSource<OtpAccount?>();
    }

    protected override void OnDisappearing()
    {
        _pageResultCompletionSource.TrySetResult(null);
    }

    public Task<OtpAccount?> WaitForResultAsync() => _pageResultCompletionSource.Task;

    private void dataForm_ValidateProperty(object sender, DevExpress.Maui.DataForm.DataFormPropertyValidationEventArgs e)
    {
        if (e.PropertyName == nameof(OtpObservable.Description))
        {
            if (string.IsNullOrEmpty(e.NewValue?.ToString()))
            {
                e.HasError = true;
                e.ErrorText = "Description is required";
            }
        }
        button.IsEnabled = !e.HasError;
    }

    private async void Save_Clicked(object sender, EventArgs e)
    {
        var isValid = dataForm.Validate();
        if (!isValid) return;
        _pageResultCompletionSource.TrySetResult(dataForm.DataObject as OtpAccount);
        await Navigation.PopAsync().ConfigureAwait(true);
    }

    private async void OnEntryUnfocused(object? sender, FocusEventArgs e)
    {
        // Trasla il pulsante verso l'alto quando la tastiera è visibile
        await button.TranslateTo(0, 0, 250, Easing.CubicInOut);
    }

    private async void OnEntryFocused(object? sender, FocusEventArgs e)
    {
        // Riporta il pulsante alla posizione originale quando la tastiera è nascosta
        await button.TranslateTo(0, 700, 250, Easing.CubicInOut);
    }
}