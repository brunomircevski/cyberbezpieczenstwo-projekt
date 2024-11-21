using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Channels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models.Dto;

public class PlanDto()
{
    public string Name { get; set; }

    public int Days { get; set; }

    public double Price { get; set; }

    public double Discount { get; set; }
}
