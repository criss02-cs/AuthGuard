using BarcodeScanning;
using System.Threading.Tasks;

namespace Otp.Maui.Views;

public partial class ScannerView : ContentPage
{
    private readonly TaskCompletionSource<BarcodeResult?> _pageResultCompletionSource;
    private CameraView? _cameraView;

    public ScannerView()
    {
        InitializeComponent();
        _pageResultCompletionSource = new TaskCompletionSource<BarcodeResult?>();
    }

    protected override bool OnBackButtonPressed()
    {
        _pageResultCompletionSource.TrySetResult(null);
        return false;
    }

    public Task<BarcodeResult?> WaitForResultAsync() => _pageResultCompletionSource.Task;

    private void TorchButton_Clicked(object? sender, EventArgs e)
    {
        _cameraView!.TorchOn = !_cameraView.TorchOn;
    }

    private async void ScannerView_OnLoaded(object sender, EventArgs e)
    {
        await Methods.AskForRequiredPermissionAsync();
        var task = new Task<CameraView>(() =>
        {
            Padding = new Thickness(10, 0, 10, 0);
            _cameraView = new CameraView
            {
                BarcodeSymbologies = BarcodeFormats.QRCode,
                VibrationOnDetected = true,
                CameraEnabled = true
            };
            _cameraView.WidthRequest = _cameraView.HeightRequest = 300;
            _cameraView.OnDetectionFinished += BarcodeView_OnDetectionFinished;
            _cameraView.Margin = new Thickness(2);
            return _cameraView;
        });
        task.Start();
        var camera = await task;
        camera.CameraEnabled = true;
        MainThread.BeginInvokeOnMainThread(() => CameraViewGrid.Add(camera));
    }

    private async void BarcodeView_OnDetectionFinished(object? sender, OnDetectionFinishedEventArg e)
    {
        if (e.BarcodeResults.Count == 0) return;
        var result = e.BarcodeResults.ElementAt(0);
        _pageResultCompletionSource.TrySetResult(result);
        await Navigation.PopAsync(true);
    }

    private async void CloseButton_Clicked(object? sender, EventArgs e)
    {
        _pageResultCompletionSource.TrySetResult(null);
        await Navigation.PopAsync();
    }
}