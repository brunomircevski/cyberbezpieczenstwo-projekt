using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models;

public class Stream()
{
    [BsonIgnore]
    [Key]
    public string Id { get; set; }

    public string Name { get; set; }
    
    public string SavedPath { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    [BsonIgnore]
    public Channel Channel { get; set; }
}