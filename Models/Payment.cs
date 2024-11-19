using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Channels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models;

public class Payment()
{
    [BsonIgnore]
    [Key]
    public string Id { get; set; }

    public DateTime PaymentDate { get; set; }

    public int AddedDays { get; set; }

    public double FullPrice { get; set; }

    public double PaidPrice { get; set; }

    public double Discount { get; set; }

    [BsonIgnore]
    public Subscription Subscription { get; set; }
}
