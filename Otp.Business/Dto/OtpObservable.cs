using CommunityToolkit.Mvvm.ComponentModel;
using MongoDB.Bson;
using Otp.Business.Models;
using Otp.Business.Services;

namespace Otp.Business.Dto;

public partial class OtpObservable(OtpAccount? otp = null) : ObservableObject
{
    [ObservableProperty] private ObjectId _id = otp?.Id ?? ObjectId.GenerateNewId();
    [ObservableProperty] private string _secret = otp?.Secret ?? "";
    [ObservableProperty] private string _issuer = otp?.Issuer ?? "";
    [ObservableProperty] private string _description = otp?.Description ?? "";
    [ObservableProperty] private DateTimeOffset _insertedAt = otp?.InsertedAt ?? DateTimeOffset.Now;
    [ObservableProperty] private DateTimeOffset _updatedAt = otp?.UpdatedAt ?? DateTimeOffset.Now;
    [ObservableProperty] private string _otp = OtpService.GenerateOtp(otp?.Secret ?? "").Otp;
    [ObservableProperty] private int _timeRemaining = OtpService.GenerateOtp(otp?.Secret ?? "").RemainingSeconds;
}