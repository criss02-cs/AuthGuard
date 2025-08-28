// See https://aka.ms/new-console-template for more information

using Otp.Business.Models;
using Otp.Business.Services;
using Otp.Business.ViewModels;
using OtpNet;
using System.Text.RegularExpressions;

Console.WriteLine("Hello, World!");

var result = OtpService.DecodeOtpMigrationUrl("otpauth-migration://offline?data=ClsKFESQJCFW5UWz3WlCLmuYIn0EmkSLEgxFYXN5IHByb2plY3QaGmdic29mdHdhcmUuZWFzeXByb2plY3QuY29tIAEoATACQhM2NDBjZmIxNzA2MDAxNzk1NjY0EAIYASAA");
foreach (var otp in result)
{
    var otpAccount = new OtpAccount
    {
        Secret = Base32Encoding.ToString(otp.Secret),
        Description = $"{otp.Name} - {otp.Issuer}",
        Issuer = otp.Issuer,
        InsertedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };
    Console.WriteLine($"Secret: {otpAccount.Secret}, Description: {otpAccount.Description}, Issuer: {otpAccount.Issuer}");
}
var regex = @"otpauth:\/\/totp\/[^?]+\?secret=([A-Za-z2-7]+)";
var match = Regex.Match("otpauth://totp/GBinWeb?secret=33u5gmv6z7hy5jbommt4dpsvwjqnup4pmhn6zigtmlubvvyicsdcm5kn", regex);
if (match.Success)
{
    Console.WriteLine(match.Groups[1].Value);
}

Console.ReadKey();