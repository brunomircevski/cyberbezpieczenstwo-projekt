using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models;

public class Message()
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [Key]
    public string Id { get; set; }

    public string Content { get; set; }

    public DateTime Date { get; set; }

    [BsonIgnore]
    public User Sender { get; set; }

    [NotMapped]
    public string SenderId { get; set; }

    [BsonIgnore]
    public User Recipient { get; set; }

    [NotMapped]
    public string RecipientId { get; set; }
}