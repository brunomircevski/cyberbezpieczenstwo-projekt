using Cyberbezpieczenstwo.Models;
using Cyberbezpieczenstwo.Data;
using System.Security.Principal;

namespace Cyberbezpieczenstwo.Services;

public class LogService
{
    private readonly string logFilePath = "login_logs.txt";

    public void LogNonExistingUserLoginAttempt(string username)
    {
        string message = $"[{DateTime.Now}] LOGIN ATTEMPT for NON-EXISTING USER: {username}";
        File.AppendAllText(logFilePath, message + Environment.NewLine);
    }

    public void LogFailedLogin(string username, DateTime timestamp)
    {
        string message = $"[{timestamp}] FAILED LOGIN for user: {username}";
        File.AppendAllText(logFilePath, message + Environment.NewLine);
    }

    public void LogSuccessfulLogin(string username, DateTime timestamp)
    {
        string message = $"[{timestamp}] SUCCESSFUL LOGIN for user: {username}";
        File.AppendAllText(logFilePath, message + Environment.NewLine);
    }

    public void LogFailedLoginWithCounter(string username, int failedCount, DateTime timestamp)
    {
        string message = $"[{timestamp}] FAILED LOGIN for user: {username}. " +
                         $"Failed attempts since last success: {failedCount}";
        File.AppendAllText(logFilePath, message + Environment.NewLine);
    }

}