using CommunityToolkit.Maui.Core.Extensions;
using Microsoft.Maui.Platform;

namespace Otp.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        LoadAccentColor();
        //MainPage = new AppShell();
    }

    protected override Window CreateWindow(IActivationState? activationState) => new(new AppShell());

    private void LoadAccentColor()
    {
#if ANDROID
        var value = new Android.Util.TypedValue();
        Android.App.Application.Context.ApplicationContext!.Theme!.ResolveAttribute(Android.Resource.Attribute.ColorAccent, value, true);
        var contexwrapper = new AndroidX.AppCompat.View.ContextThemeWrapper(Android.App.Application.Context.ApplicationContext, Android.Resource.Style.ThemeDeviceDefault);
        contexwrapper.Theme!.ResolveAttribute(Android.Resource.Attribute.ColorAccent, value, true);
        var color = value.Data;
        var mauicolor = new Android.Graphics.Color(color).ToColor();
        //Microsoft.Maui.MauiContext.Current.Resources["AccentColor"] = mauicolor;
        Resources.TryAdd("AccentColor", mauicolor);
        var isDark = mauicolor.IsDark();
        Resources.TryAdd("TextAccentColor", isDark ? Color.FromRgb(255, 255, 255) : Color.FromRgb(0, 0, 0));
#elif WINDOWS
        //var uiSettings = new Windows.UI.ViewManagement.UISettings();
        //var color = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
        //var mauicolor = Color.Parse(color.ToString());
#endif
    }
}