using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

    [BsonIgnore]
    public Channel Channel { get; set; }

    public List<Comment> Comments { get; set; }

    public List<Rating> Ratings { get; set; }

    public List<Image> Images { get; set; }
    public List<Attachment> Attachments { get; set; }
}