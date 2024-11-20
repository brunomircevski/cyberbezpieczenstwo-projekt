using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models.Dto;

public class UserDto()
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }
}
