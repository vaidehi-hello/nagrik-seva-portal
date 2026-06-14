namespace NagrikSevaAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsEmailVerified { get; set; } = false;
        public string? VerificationToken { get; set; }
        public DateTime? VerificationTokenExpiry { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<UserRole> UserRoles { get; set; }
            = new List<UserRole>();
    }
}