using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models.Dto;

public class LiveStreamDto()
{
    public string Name { get; set; }
    
    public string SavedPath { get; set; }

    public DateTime EndDate { get; set; }
}