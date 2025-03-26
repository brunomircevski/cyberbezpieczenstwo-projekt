using Cyberbezpieczenstwo.Models;
using Cyberbezpieczenstwo.Data;
using System.Security.Principal;
using BCrypt.Net;

namespace Cyberbezpieczenstwo.Services;

public class PasswordService
{
    public List<PasswordFragment> GenerateFragments(string password)
    {
        if (password.Length < 8 || password.Length > 16)
            throw new Exception("Password must be 8-16 characters long!");

        var random = new Random();
        var fragments = new List<PasswordFragment>();
        var uniqueFragments = new HashSet<string>(); // Store unique fragments

        while (fragments.Count < 10)
        {
            var indexes = new HashSet<int>();
            while (indexes.Count < 5)
            {
                indexes.Add(random.Next(1, password.Length + 1));
            }

            var sortedIndexes = indexes.OrderBy(x => x).ToList();
            var fragment = new string(sortedIndexes.Select(idx => password[idx - 1]).ToArray());

            // Ensure fragment is unique
            if (!uniqueFragments.Contains(fragment))
            {
                uniqueFragments.Add(fragment);
                var hash = BCrypt.Net.BCrypt.HashPassword(fragment);

                fragments.Add(new PasswordFragment
                {
                    Pattern = string.Join(",", sortedIndexes),
                    Hash = hash
                });
            }
        }

        return fragments;
    }


    public bool VerifyPassword(User user, string passwordFragment)
    {
        if (user.PasswordFragments == null || user.PasswordFragments.Count == 0)
            return false;

        Console.WriteLine("PASSWORD FRAGMENT:" + passwordFragment);

        int index = user.NextFragmentIndex % user.PasswordFragments.Count;
        var fragment = user.PasswordFragments[index];

        return BCrypt.Net.BCrypt.Verify(passwordFragment, fragment.Hash);
    }
}