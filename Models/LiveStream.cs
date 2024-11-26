using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models;

public class LiveStream()
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [Key]
    public string Id { get; set; }

    public string Name { get; set; }
    
    public string SavedPath { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
    [JsonIgnore]
    [BsonIgnore]
    public string ChannelId { get; set; }

    [JsonIgnore]
    [BsonIgnore]
    public Channel Channel { get; set; }
}