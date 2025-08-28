using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Otp.Business.Dto;
using Otp.Business.Models;
using Otp.Business.Services;

namespace Otp.Business.ViewModels;

public partial class OtpListViewModel : ObservableObject
{
    [ObservableProperty]
    public partial ObservableCollection<OtpObservable> OtpList { get; set; }

    private Timer? _timer;
    [ObservableProperty] public partial string Text { get; set; }

    [RelayCommand]
    private Task LoadOtpList()
    {
        var otpList = RealmService.RealmInstance.All<OtpAccount>().ToList();
        OtpList = new ObservableCollection<OtpObservable>(otpList.Select(otp => new OtpObservable(otp)));
        _timer = new Timer(UpdateOtp, null, 0, 1000);
        return Task.CompletedTask;
    }

    private void UpdateOtp(object? state)
    {
        //Console.WriteLine("UtcNow: " + DateTime.UtcNow);
        //Console.WriteLine("Now" + DateTime.Now);
        foreach (var otp in OtpList)
        {
            var res = OtpService.GenerateOtp(otp.Secret);
            //Console.WriteLine($"{Guid.NewGuid()}: {res.Otp} - {res.RemainingSeconds}");
            otp.TimeRemaining = res.RemainingSeconds;
            otp.Otp = res.Otp;
        }
    }

    [RelayCommand]
    private async Task CameraTappedAsync(OtpAccount otp)
    {
        await RealmService.RealmInstance.WriteAsync(() =>
        {
            RealmService.RealmInstance.Add(otp);
        });
        OtpList.Add(new OtpObservable(otp));
        //LoadOtpListCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task RemoveOtp(OtpObservable otp)
    {
        var otpDb = RealmService.RealmInstance.Find<OtpAccount>(otp.Id);
        if (otpDb == null) return;
        await RealmService.RealmInstance.WriteAsync(() =>
        {
            RealmService.RealmInstance.Remove(otpDb);
        });
        OtpList.Remove(OtpList.First(o => o.Id == otp.Id));
    }

    [RelayCommand]
    private async Task UpdateOtp(OtpAccount otp)
    {
        var otpObs = OtpList.FirstOrDefault(x => x.Secret == otp.Secret);
        if (otpObs == null) return;
        var otpDb = RealmService.RealmInstance.Find<OtpAccount>(otpObs.Id);
        if (otpDb == null) return;
        await RealmService.RealmInstance.WriteAsync(() =>
        {
            otpDb.Description = otp.Description;
        });
        await LoadOtpList();
    }

    [RelayCommand]
    private void SearchOtp()
    {
        var result = RealmService.RealmInstance
            .All<OtpAccount>()
            .Where(x => x.Issuer.Contains(Text, StringComparison.OrdinalIgnoreCase))
            .ToList();
        OtpList = new ObservableCollection<OtpObservable>(result.Select(otp => new OtpObservable(otp)));
    }
}