using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Channels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models.Dto;

public class SubscriptionDto()
{
    public DateTime EndDate { get; set; }
    
    public DateTime StartDate { get; set; }

    public Boolean AutoRenew { get; set; }

    public string ChannelId { get; set; }
}
