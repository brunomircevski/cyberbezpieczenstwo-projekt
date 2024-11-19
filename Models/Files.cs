using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models;

public class File()
{
    [BsonIgnore]
    [Key]
    public string Id { get; set; }

    public string Name { get; set; }

    public string Path { get; set; }

    public int Size { get; set; }

    [BsonIgnore]
    public Post Post { get; set; }
}