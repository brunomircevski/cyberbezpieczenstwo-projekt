using Cyberbezpieczenstwo.Models;
using Cyberbezpieczenstwo.Data;
using System.Security.Principal;

namespace Cyberbezpieczenstwo.Services;

public class AccountService
{
    private readonly Dictionary<string, int> _tokenUserIdDictionary = [];

    public string GetToken(int userId) {
        string token = Guid.NewGuid().ToString("N");
        _tokenUserIdDictionary.Add(token, userId);
        return token;
    }

    public int GetUserId(string token) {
        return _tokenUserIdDictionary.TryGetValue(token, out int userId) ? userId : 0;
    }

    public void Logout(string token) {
        _tokenUserIdDictionary.Remove(token);
    }
}