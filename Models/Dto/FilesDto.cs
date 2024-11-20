using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models.Dto;

public class FileDto()
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [Key]
    public string Id { get; set; }

    public string Name { get; set; }

    public string Path { get; set; }

    public int Size { get; set; }

    [BsonIgnore]
    public Post Post { get; set; }
}