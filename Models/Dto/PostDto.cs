using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models.Dto;

public class PostDto()
{
    public string Title { get; set; }

    public string Content { get; set; }

    public Boolean IsSponsored { get; set; }
}