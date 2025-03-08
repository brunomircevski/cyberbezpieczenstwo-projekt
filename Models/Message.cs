using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cyberbezpieczenstwo.Models;

public class Message()
{
    [Key]
    public int Id { get; set; }

    public string Content { get; set; }

    public DateTime Date { get; set; }
    
    [JsonIgnore]
    public User Sender { get; set; }

    public int SenderId { get; set; }

    [JsonIgnore]
    public List<User> Editors { get; set; } = [];

    [NotMapped]
    [JsonInclude]
    public List<int> EditorsId => Editors.Select(e => e.Id).ToList() ?? [];

}
