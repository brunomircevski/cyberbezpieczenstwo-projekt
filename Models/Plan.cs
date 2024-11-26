using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models;

public class Plan()
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [Key]
    public string Id { get; set; }

    public string Name { get; set; }

    public int Days { get; set; }

    public double Price { get; set; }

    public double Discount { get; set; }
    [JsonIgnore]
    [BsonIgnore]
    public string ChannelId { get; set; }

    [JsonIgnore]
    [BsonIgnore]
    public Channel Channel { get; set; }
}
