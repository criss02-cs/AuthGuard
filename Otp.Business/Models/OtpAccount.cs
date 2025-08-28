using MongoDB.Bson;
using Realms;

namespace Otp.Business.Models;

public partial class OtpAccount : RealmObject
{
    [PrimaryKey, MapTo("_id")]
    public ObjectId Id { get; private set; } = ObjectId.GenerateNewId();

    public string Secret { get; set; } = "";
    public string Description { get; set; } = "";
    public string Issuer { get; set; } = "";
    public DateTimeOffset InsertedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}