using System.ComponentModel.DataAnnotations;

namespace Cyberbezpieczenstwo.Models;

public class User()
{
    [Key]
    public int Id { get; set; }

    public string Login { get; set; }

    public string Password { get; set; }

    public List<Message> Messages { get; set; }
}
