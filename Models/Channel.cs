using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models;

public class Channel()
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [Key]
    public string Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public DateTime CreationDate { get; set; }

    [JsonIgnore]
    [BsonIgnore]
    public List<Subscription> Subscriptions { get; set; }

    [JsonIgnore]
    public List<Category> Categories { get; set; }

    [JsonIgnore]
    public List<Post> Posts { get; set; }

    [JsonIgnore]
    public List<LiveStream> Streams { get; set; }

    [JsonIgnore]
    public List<Plan> Plans { get; set; }

    [JsonIgnore]
    [BsonIgnore]
    public User Owner { get; set; }

    [NotMapped]
    public string OwnerId { get; set; }
}