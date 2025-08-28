using CommunityToolkit.Mvvm.Input;
using Otp.Business.Models;
using Otp.Maui.Utils;
using Otp.Maui.Views;
using OtpNet;
using System.Text.RegularExpressions;
using Otp.Business.Services;
using QRCoder;

namespace Otp.Maui.Components;

public partial class MenuButton : ContentView
{
    private const string OPEN_MENU_ANIM_NAME = "openmenu";
    private const string CLOSE_MENU_ANIM_NAME = "openmenu";
    private bool _isOpen;

    public MenuButton()
    {
        InitializeComponent();
        menuitem1.Command = new AsyncRelayCommand(x => Camera_Tapped());
        menuitem2.Command = new AsyncRelayCommand(x => Keyboard_Tapped());
        menuitem3.Command = new AsyncRelayCommand(x => ImportTapped());
        menuitem4.Command = new AsyncRelayCommand(x => ExportTapped());
        Popup.PlacementTarget = mainbutton;
        Popup.Placement = DevExpress.Maui.Core.Placement.Top;
    }

    public event EventHandler<OtpAccount>? CameraTapped;

    private void OpenMenu()
    {
        _isOpen = true;
        if (this.AnimationIsRunning(CLOSE_MENU_ANIM_NAME))
        {
            this.AbortAnimation(CLOSE_MENU_ANIM_NAME);
        }
        var animation = new Animation
        {
            {0,0.5, new Animation(r => bars.Rotation = r, 0, 90) },
            {0,0.5, new Animation(m => bars.WidthRequest = m, 5,25)},
        };
        animation.Commit(this, OPEN_MENU_ANIM_NAME, length: 2000, easing: Easing.SpringOut);
    }

    private void CloseMenu()
    {
        _isOpen = false;

        if (this.AnimationIsRunning(OPEN_MENU_ANIM_NAME))
            this.AbortAnimation(OPEN_MENU_ANIM_NAME);

        var animation = new Animation
        {
            {0,0.5, new Animation(r => bars.Rotation = r, 90, 0) },
            {0,0.5, new Animation(m => bars.WidthRequest = m, 25,5)},
        };
        animation.Commit(this, CLOSE_MENU_ANIM_NAME, length: 2000, easing: Easing.SpringOut);
    }

    public void Animate()
    {
        if (_isOpen)
        {
            CloseMenu();
            return;
        }

        OpenMenu();
    }

    private void mainbutton_Tapped(object sender, TappedEventArgs e)
    {
        Animate();
        if (Popup.IsOpen)
        {
            Popup.IsOpen = false;
            return;
        }
        Popup.IsOpen = true;
        //Popup.Placement = DevExpress.Maui.Core.Placement.Top;
        //Popup.PlacementTarget = sender as View;
        //Camera_Tapped();
    }

    private async Task Keyboard_Tapped()
    {
        try
        {
            Popup.IsOpen = false;
            var newOtpPage = new NewOtpView();
            await Navigation.PushAsync(newOtpPage, true);
            var otpAccount = await newOtpPage.WaitForResultAsync();
            if (otpAccount == null) return;
            //otpAccount.Secret = Base32Encoding.e(otpAccount.Secret);
            CameraTapped?.Invoke(this, otpAccount);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await MauiAlert.Alert(ex.Message);
        }
    }

    private async Task ExportTapped()
    {
        try
        {
            Popup.IsOpen = false;
            var otpAccounts = RealmService.RealmInstance.All<OtpAccount>().ToList();
            var otpMigration = OtpService.GenerateExportOtpUrl(otpAccounts);
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(otpMigration, QRCodeGenerator.ECCLevel.L);
            PngByteQRCode qRCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeBytes = qRCode.GetGraphic(20);
            ImageSource qrImageSource = ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));
            QrCodeImage.Source = qrImageSource;
            //Barcode.BarcodeEncoder = new ZXingBarcodeEncoder();
            //Barcode.Barcode = otpMigration;
            BarcodePopup.IsOpen = true;
            //QrCodeImage.Source
        }
        catch (Exception ex)
        {
            await MauiAlert.Alert(ex.Message);
        }
    }

    private async Task ImportTapped()
    {
        try
        {
            Popup.IsOpen = false;
            var barcodePage = new ScannerView();
            await Navigation.PushAsync(barcodePage, true);
            var result = await barcodePage.WaitForResultAsync();
            if (result == null) return;
            if (!result.RawValue.StartsWith("otpauth-migration")) return;
            var otps = OtpService.DecodeOtpMigrationUrl(result.RawValue);
            foreach (var otp in otps)
            {
                var otpAccount = new OtpAccount
                {
                    Secret = Base32Encoding.ToString(otp.Secret),
                    Description = $"{otp.Name} - {otp.Issuer}",
                    Issuer = otp.Issuer,
                    InsertedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
                CameraTapped?.Invoke(this, otpAccount);
            }
        }
        catch (Exception ex)
        {
            await MauiAlert.Alert(ex.Message);
        }
    }

    private async Task Camera_Tapped()
    {
        try
        {
            Popup.IsOpen = false;
            var barcodePage = new ScannerView();
            await Navigation.PushAsync(barcodePage, true);
            var result = await barcodePage.WaitForResultAsync();
            if (result == null) return;
            var secret = ExtractSecret(result.RawValue);
            var otpAccount = new OtpAccount
            {
                Secret = secret,
                Description = "New Account",
                Issuer = ExtractIssuer(result.RawValue),
                InsertedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            var accountName = await MauiAlert.ShowInputDialog("OTP Guard", "Inserisci il nome dell'account");
            if (string.IsNullOrEmpty(accountName)) return;
            otpAccount.Description = accountName;
            CameraTapped?.Invoke(this, otpAccount);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await MauiAlert.Alert(ex.Message);
        }
    }

    private string ExtractSecret(string barcode)
    {
        var match = Regex.Match(barcode, @"otpauth:\/\/totp\/[^?]+\?secret=([A-Za-z2-7]+)");
        if (match.Success)
        {
            return match.Groups[1].Value;
        }
        return barcode;
    }

    private string ExtractIssuer(string barcode)
    {
        var match = Regex.Match(barcode, @"otpauth:\/\/totp\/[^?]+\?issuer=([^&]+)");
        if (match.Success)
        {
            return match.Groups[1].Value;
        }
        return "";
    }

    private void Popup_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        Animate();
    }
}