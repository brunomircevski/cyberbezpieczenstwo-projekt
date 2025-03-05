using System.ComponentModel.DataAnnotations;

namespace Cyberbezpieczenstwo.Models;

public class Message()
{
    [Key]
    public int Id { get; set; }

    public string Content { get; set; }

    public DateTime Date { get; set; }

    public User Sender { get; set; }
}