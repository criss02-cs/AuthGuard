using System.Net;
using System.Text;
using Otp.Business.Models;
using OtpNet;
using ProtoBuf;

namespace Otp.Business.Services;

public class OtpService
{
    public static (string Otp, int RemainingSeconds) GenerateOtp(string secret)
    {
        try
        {
            var totp = new Totp(Base32Encoding.ToBytes(secret));
            return (totp.ComputeTotp(), totp.RemainingSeconds());
        }
        catch (Exception)
        {
            return ("", 0);
        }
    }

    public static string GenerateSecretKey(int length = 32)
    {
        var key = KeyGeneration.GenerateRandomKey(length);
        return Base32Encoding.ToString(key);
    }

    public static string GenerateExportOtpUrl(List<OtpAccount> codes)
    {
        try
        {
            var payload = new Payload();
            foreach (var code in codes)
            {
                payload.OtpParameters.Add(new OtpParameters
                {
                    Secret = Base32Encoding.ToBytes(code.Secret),
                    Name = code.Description,
                    Issuer = code.Issuer,
                    Algorithm = 1, // SHA1 (Valore predefinito)
                    Digits = 6, // 6 cifre (Valore predefinito)
                    Type = 2, // TOTP (Valore predefinito)
                    Counter = 0 // non usato per TOTP
                });
            }
            using var stream = new MemoryStream();
            Serializer.Serialize(stream, payload);
            var protoData = stream.ToArray();
            var base64Data = Convert.ToBase64String(protoData)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
            return $"otpauth-migration://offline?data={base64Data}";
        }
        catch (Exception ex)
        {
            throw new Exception("Error generating OTP migration URL", ex);
        }
    }

    public static List<OtpParameters> DecodeOtpMigrationUrl(string url)
    {
        try
        {
            // Decodifica l'URL e ottieni i dati Base64
            string decodedUrl = WebUtility.UrlDecode(url);
            string base64Data = decodedUrl.Split("data=")[1]
                .Replace("-", "+")
                .Replace("_", "/"); ;

            // Aggiungi padding mancante
            // Padding
            while (base64Data.Length % 4 != 0)
                base64Data += "=";
            byte[] protoData = Convert.FromBase64String(base64Data);

            // Deserializza i dati usando protobuf-net
            using var stream = new MemoryStream(protoData);
            stream.Position = 0;
            var payload = Serializer.Deserialize<Payload>(stream);
            return payload.OtpParameters;
        }
        catch (Exception ex)
        {
            throw new Exception("Error decoding OTP migration URL", ex);
        }
    }
}

[ProtoContract]
public class Payload
{
    [ProtoMember(1, IsRequired = true)]
    public List<OtpParameters> OtpParameters { get; set; } = [];
}

[ProtoContract]
public class OtpParameters
{
    [ProtoMember(1)]
    public byte[] Secret { get; set; } = [];

    [ProtoMember(2)]
    public string Name { get; set; } = "";

    [ProtoMember(3)]
    public string Issuer { get; set; } = "";

    [ProtoMember(4)]
    public int Algorithm { get; set; } // Usa int, non enum

    [ProtoMember(5)]
    public int Digits { get; set; }

    [ProtoMember(6)]
    public int Type { get; set; } // Usa int, non enum

    [ProtoMember(7)]
    public long Counter { get; set; }
}