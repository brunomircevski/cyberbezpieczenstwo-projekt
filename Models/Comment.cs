using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models;

public class Comment()
{
    [BsonIgnore]
    [Key]
    public string Id { get; set; }

    public string Content { get; set; }

    public DateTime CreationDate { get; set; }

    [BsonIgnore]
    public Post Post { get; set; }

    [BsonIgnore]
    public User Author { get; set; }

    [NotMapped]
    public string AuthorId { get; set; }
}