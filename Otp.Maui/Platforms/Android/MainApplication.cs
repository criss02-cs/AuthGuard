using Android.App;
using Android.Runtime;

[assembly:
    UsesPermission(Android.Manifest.Permission.AccessNetworkState),
    UsesPermission(Android.Manifest.Permission.UseBiometric),
    UsesPermission(Android.Manifest.Permission.UseFingerprint),
    UsesPermission(Android.Manifest.Permission.Camera),
    UsesPermission(Android.Manifest.Permission.Vibrate)]
namespace Otp.Maui;

[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}