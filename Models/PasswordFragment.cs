using System.ComponentModel.DataAnnotations;

namespace Cyberbezpieczenstwo.Models;

public class PasswordFragment()
{
    [Key]
    public int Id { get; set; }

    public string Pattern { get; set; }

    public string Hash { get; set; }
}
