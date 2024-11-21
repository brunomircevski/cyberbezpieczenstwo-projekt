using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models;

public class Comment()
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [Key]
    public string Id { get; set; }

    public string Content { get; set; }

    public DateTime CreationDate { get; set; }

    [JsonIgnore]
    [BsonIgnore]
    public Post Post { get; set; }

    [JsonIgnore]
    [BsonIgnore]
    public User Author { get; set; }

    [NotMapped]
    public string AuthorId { get; set; }
}