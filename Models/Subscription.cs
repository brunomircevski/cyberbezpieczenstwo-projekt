using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Channels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models;

public class Subscription()
{
    [BsonIgnore]
    [Key]
    public string Id { get; set; }

    public DateTime EndDate { get; set; }
    
    public DateTime StartDate { get; set; }

    public Boolean AutoRenew { get; set; }

    [BsonIgnore]
    public User User { get; set; }

    [BsonIgnore]
    public Channel Channel { get; set; }

    [NotMapped]
    public int ChannelId { get; set; }

    public List<Payment> Payments { get; set; }
}
