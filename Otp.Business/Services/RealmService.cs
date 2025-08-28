using Otp.Business.Models;
using Realms;

namespace Otp.Business.Services;

public class RealmService
{
    private static readonly RealmConfiguration Config = new("otp.realm");

    public static Realm RealmInstance => Realm.GetInstance(GetRealmConfiguration());

    private static RealmConfiguration GetRealmConfiguration()
    {
        Config.Schema = new Type[]
        {
            typeof(OtpAccount)
        };
        Config.ShouldDeleteIfMigrationNeeded = true;
        return Config;
    }
}