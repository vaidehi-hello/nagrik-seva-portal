using NagrikSevaAPI.Models;

namespace NagrikSevaAPI.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByVerificationTokenAsync(string token);
        Task<User> RegisterAsync(User user);
        Task UpdateUserAsync(User user);
        Task AddUserRoleAsync(UserRole userRole);
        Task<string> GetUserRoleAsync(int userId);
        Task<List<User>> GetAllUsersAsync();
    }
}