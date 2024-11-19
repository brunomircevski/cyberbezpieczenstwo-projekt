using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models;

public class Rating()
{
    [BsonIgnore]
    [Key]
    public string Id { get; set; }

    public int Value { get; set; }

    [BsonIgnore]
    public Post Post { get; set; }

    [BsonIgnore]
    public User User { get; set; }

    [NotMapped]
    public string UserId { get; set; }
}