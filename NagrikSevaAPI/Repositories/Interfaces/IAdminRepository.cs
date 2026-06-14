using NagrikSevaAPI.Models;

namespace NagrikSevaAPI.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<List<User>> GetAllUsersAsync();
        Task<List<User>> GetUsersByRoleAsync(string role);
        Task<bool> DeleteUserAsync(int id);
        Task<Announcement> CreateAnnouncementAsync(Announcement announcement);
        Task<List<Announcement>> GetAnnouncementsAsync();
        Task<bool> DeleteAnnouncementAsync(int id);
        Task<Dictionary<string, int>> GetDashboardStatsAsync();
    }
}