using System.Text.RegularExpressions;

namespace NagrikSevaAPI.Helpers
{
    public static class ValidationHelper
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }

        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            if (password.Length < 8) return false;
            if (!password.Any(char.IsUpper)) return false;
            if (!password.Any(char.IsLower)) return false;
            if (!password.Any(char.IsDigit)) return false;
            var special = "!@#$%^&*()_+-=[]{}|;:,.<>?";
            if (!password.Any(c => special.Contains(c))) return false;
            return true;
        }

        public static string GetPasswordError(string password)
        {
            if (password.Length < 8) return "Minimum 8 characters required!";
            if (!password.Any(char.IsUpper)) return "Add at least one uppercase letter!";
            if (!password.Any(char.IsLower)) return "Add at least one lowercase letter!";
            if (!password.Any(char.IsDigit)) return "Add at least one number!";
            return "Add at least one special character (!@#$%)!";
        }
    }
}