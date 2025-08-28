using BarcodeScanning;
using CommunityToolkit.Maui.Core.Platform;
using Otp.Business.Dto;
using Otp.Business.Models;
using Otp.Business.Services;
using System.Threading.Tasks;

namespace Otp.Maui.Views;

public partial class NewOtpView : ContentPage
{
    private readonly TaskCompletionSource<OtpAccount?> _pageResultCompletionSource;

    public NewOtpView(OtpAccount? otp = null)
    {
        InitializeComponent();
        dataForm.DataObject = otp ?? new OtpAccount();
        _pageResultCompletionSource = new TaskCompletionSource<OtpAccount?>();
        button.IsEnabled = false;
        // Aggiungi gestori di eventi per la tastiera
        //dataForm.Focused += OnEntryFocused;
        //dataForm.Unfocused += OnEntryUnfocused;
    }

    protected override void OnDisappearing()
    {
        _pageResultCompletionSource.TrySetResult(null);
    }

    public Task<OtpAccount?> WaitForResultAsync() => _pageResultCompletionSource.Task;

    private void dataForm_ValidateProperty(object sender, DevExpress.Maui.DataForm.DataFormPropertyValidationEventArgs e)
    {
        if (e.PropertyName == nameof(OtpObservable.Secret))
        {
            if (string.IsNullOrEmpty(e.NewValue?.ToString()))
            {
                e.HasError = true;
                e.ErrorText = "Secret is required";
            }
        }
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

    private void GenerateRandom_Clicked(object sender, EventArgs e)
    {
        if (dataForm.DataObject is not OtpAccount otp)
        {
            return;
        }
        otp.Secret = OtpService.GenerateSecretKey();
    }
}