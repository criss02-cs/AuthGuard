using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Platform;
using CommunityToolkit.Maui.Views;
using Otp.Business.Dto;
using Otp.Business.Models;
using Otp.Business.ViewModels;
using Otp.Maui.Utils;

namespace Otp.Maui.Views;

public partial class OtpListView : ContentPage
{
    public OtpListView(OtpListViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        ChangeStyleOfStatusBar();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as OtpListViewModel)?.LoadOtpListCommand.ExecuteAsync(null);
    }

    private void ChangeStyleOfStatusBar()
    {
        var accentColor = Application.Current!.Resources.TryGetValue("AccentColor", out var color) ? (Color)color : Color.FromArgb("#0078D4");
        var isLight = accentColor.IsLight();
        var statusBarStyle = isLight ? StatusBarStyle.DarkContent : StatusBarStyle.LightContent;
        StatusBar.SetStyle(statusBarStyle);
        StatusBar.SetColor(accentColor);
        //StartContainer
        var template = OtpList.ItemTemplate;
    }

    private void MenuButton_CameraTapped(object sender, Business.Models.OtpAccount e)
    {
        (BindingContext as OtpListViewModel)?.CameraTappedCommand.ExecuteAsync(e);
    }

    private async void SwipeContainerItem_Tap(object sender, DevExpress.Maui.CollectionView.SwipeItemTapEventArgs e)
    {
        var item = e.Item as OtpObservable;
        var result = await MauiAlert.ShowConfirmDialog("Delete", "Do you want to delete this item?");
        if (!result) return;
        (BindingContext as OtpListViewModel)?.RemoveOtpCommand.ExecuteAsync(item);
    }

    private async void ModifySwipeContainerItem_Tap(object sender, DevExpress.Maui.CollectionView.SwipeItemTapEventArgs e)
    {
        try
        {
            var account = e.Item as OtpObservable;
            if (account == null) return;
            var otp = new OtpAccount
            {
                Description = account.Description,
                Secret = account.Secret,
                Issuer = account.Issuer,
                //Id = account.Id,
                InsertedAt = account.InsertedAt,
            };
            var updateOtpPage = new UpdateOtpView(otp);
            await Navigation.PushAsync(updateOtpPage, true);
            var otpAccount = await updateOtpPage.WaitForResultAsync();
            if (otpAccount == null) return;
            //otpAccount.Secret = Base32Encoding.e(otpAccount.Secret);
            (BindingContext as OtpListViewModel)?.UpdateOtpCommand.ExecuteAsync(otpAccount);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await MauiAlert.Alert(ex.Message);
        }
    }
}

public static class ColorExtensions
{
    public static bool IsLight(this Color color)
    {
        // Convert the color to sRGB
        double r = color.Red;
        double g = color.Green;
        double b = color.Blue;

        // Apply the sRGB luminance formula
        double luminance = 0.2126 * r + 0.7152 * g + 0.0722 * b;

        // A threshold of 0.5 is commonly used to determine if a color is light or dark
        return luminance > 0.5;
    }
}