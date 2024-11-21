using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models;

public class Rating()
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [Key]
    public string Id { get; set; }

    public int Value { get; set; }

    [JsonIgnore]
    [BsonIgnore]
    public Post Post { get; set; }

    [JsonIgnore]
    [BsonIgnore]
    public User Author { get; set; }

    [JsonIgnore]
    [NotMapped]
    public string AuthorId { get; set; }
}