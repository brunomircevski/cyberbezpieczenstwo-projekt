using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models.Dto;

public class ChannelDto()
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string OwnerId { get; set; }
}