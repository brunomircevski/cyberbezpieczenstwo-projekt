using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models;

public class Post()
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [Key]
    public string Id { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public DateTime CreationDate { get; set; }

    public Boolean IsSponsored { get; set; }
    [JsonIgnore]
    [BsonIgnore]
    public string ChannelId { get; set; }

    [JsonIgnore]
    [BsonIgnore]
    public Channel Channel { get; set; }

    [JsonIgnore]
    public List<Comment> Comments { get; set; }

    [JsonIgnore]
    public List<Rating> Ratings { get; set; }

    [JsonIgnore]
    public List<Image> Images { get; set; }

    [JsonIgnore]
    public List<Attachment> Attachments { get; set; }
}