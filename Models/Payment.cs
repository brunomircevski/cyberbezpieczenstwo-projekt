using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Channels;
using Swashbuckle.AspNetCore.Annotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace BDwAS_projekt.Models;

public class Payment()
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [Key]
    public string Id { get; set; }

    public DateTime PaymentDate { get; set; }

    public int AddedDays { get; set; }

    public double FullPrice { get; set; }

    public double PaidPrice { get; set; }

    public double Discount { get; set; }
    [JsonIgnore]
    [BsonIgnore]
    public string SubscriptionId { get; set; }
    [JsonIgnore]
    [BsonIgnore]
    public Subscription Subscription { get; set; }
}