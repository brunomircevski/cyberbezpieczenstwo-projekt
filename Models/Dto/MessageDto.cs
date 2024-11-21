using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models.Dto;

public class MessageDto()
{
    public string Content { get; set; }

    public string SenderId { get; set; }

    public string RecipientId { get; set; }
}