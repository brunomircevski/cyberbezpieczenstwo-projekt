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

    [Column(TypeName = "jsonb")]
    public PaymentDetails Details { get; set; }

    public string SubscriptionId { get; set; }

    [JsonIgnore]
    [BsonIgnore]
    public Subscription Subscription { get; set; }


    public DateTime PaymentDate => Details.PaymentDate;
    public int AddedDays => Details.AddedDays;
    public double FullPrice => Details.FullPrice;
    public double PaidPrice => Details.PaidPrice;
    public double Discount => Details.Discount;

}