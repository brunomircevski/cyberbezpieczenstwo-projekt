using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Channels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models;

public class Category()
{
    [BsonIgnore]
    [Key]
    public string Id { get; set; }

    public string Name { get; set; }

    public int MinimumAge { get; set; }

    [BsonIgnore]
    public List<Channel> Channels { get; set; }
}
