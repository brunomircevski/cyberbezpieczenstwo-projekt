using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models;

public class Subscription()
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [Key]
    public string Id { get; set; }

    public DateTime EndDate { get; set; }
    
    public DateTime StartDate { get; set; }

    public Boolean AutoRenew { get; set; }

    [JsonIgnore]
    [BsonIgnore]
    public User User { get; set; }

    [JsonIgnore]
    [BsonIgnore]
    public Channel Channel { get; set; }

    [NotMapped]
    public string ChannelId { get; set; }

    [JsonIgnore]
    public List<Payment> Payments { get; set; }
}
