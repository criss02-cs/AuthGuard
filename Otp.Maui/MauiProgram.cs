using BarcodeScanning;
using CommunityToolkit.Maui;
using DevExpress.Maui;
using DevExpress.Maui.Core;
using Microsoft.Extensions.Logging;
using Otp.Business.Services;
using Otp.Business.ViewModels;
using Otp.Maui.Views;
using UraniumUI;

namespace Otp.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        ThemeManager.UseAndroidSystemColor = true;
#if IOS
        ThemeManager.Theme = new Theme(ThemeSeedColor.TealGreen);
#endif
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseDevExpress()
            .UseDevExpressGauges()
            // Initialize the .NET MAUI Community Toolkit by adding the below line of code
            .UseMauiCommunityToolkit()
            .UseDevExpressCollectionView()
            .UseDevExpressControls()
            .UseDevExpressEditors()
            .UseBarcodeScanning()
            .UseUraniumUI()
            //.UseMauiCameraView()
            .UseUraniumUIMaterial()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFontAwesomeIconFonts();
            });
        builder.Services.AddSingleton<OtpListViewModel>();
        builder.Services.AddSingleton<OtpListView>();
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}