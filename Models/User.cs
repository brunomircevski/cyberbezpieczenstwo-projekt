using System.ComponentModel.DataAnnotations;

namespace Cyberbezpieczenstwo.Models;

public class User()
{
    [Key]
    public int Id { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public List<Message> OwnMessages { get; set; } = [];

    public List<Message> EditableMessages { get; set; } = [];
}
