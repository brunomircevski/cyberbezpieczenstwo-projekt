using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Channels;
using Swashbuckle.AspNetCore.Annotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models.Dto;

public class PaymentDto()
{
    public int AddedDays { get; set; }

    public double FullPrice { get; set; }

    public double Discount { get; set; }
}