using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models.Dto;

public class ChannelDto()
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [Key]
    public string Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public DateTime CreationDate { get; set; }

    [BsonIgnore]
    public List<Subscription> Subscriptions { get; set; }

    public List<Category> Categories { get; set; }

    public List<Post> Posts { get; set; }

    public List<Post> Streams { get; set; }

    public List<Plan> Plans { get; set; }

    [BsonIgnore]
    public User Owner { get; set; }

    [NotMapped]
    public string OwnerId { get; set; }
}