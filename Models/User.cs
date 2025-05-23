using System.ComponentModel.DataAnnotations;

namespace Cyberbezpieczenstwo.Models;

public class User()
{
    [Key]
    public int Id { get; set; }

    public string Username { get; set; }

    public List<PasswordFragment> PasswordFragments { get; set; }

    public int NextFragmentIndex { get; set; }

    public List<Message> OwnMessages { get; set; } = [];

    public List<Message> EditableMessages { get; set; } = [];
    
    public bool IsLocked { get; set; }

    public int MaxFailedLogins { get; set; }

    public int FailedLoginsCounter { get; set; }

    public DateTime? LastLogin { get; set; }

    public DateTime? LastFailedLogin { get; set; }
}
