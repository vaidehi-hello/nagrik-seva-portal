using Microsoft.EntityFrameworkCore;
using NagrikSevaAPI.Data;
using NagrikSevaAPI.Models;
using NagrikSevaAPI.Repositories.Interfaces;

namespace NagrikSevaAPI.Repositories.Implementations
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .ToListAsync();
        }

        public async Task<List<User>> GetUsersByRoleAsync(string role)
        {
            return await _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Where(ur => ur.Role.Name == role)
                .Select(ur => ur.User)
                .ToListAsync();
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Announcement> CreateAnnouncementAsync(
            Announcement announcement)
        {
            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();
            return announcement;
        }

        public async Task<List<Announcement>> GetAnnouncementsAsync()
        {
            return await _context.Announcements
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> DeleteAnnouncementAsync(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null) return false;
            announcement.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Dictionary<string, int>> GetDashboardStatsAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalApplications = await _context.Applications.CountAsync();
            var pendingApplications = await _context.Applications
                .CountAsync(a => a.Status == "Pending");
            var approvedApplications = await _context.Applications
                .CountAsync(a => a.Status == "Approved");
            var rejectedApplications = await _context.Applications
                .CountAsync(a => a.Status == "Rejected");
            var totalOfficers = await _context.UserRoles
                .Include(ur => ur.Role)
                .CountAsync(ur => ur.Role.Name == "Officer");

            return new Dictionary<string, int>
            {
                { "TotalUsers", totalUsers },
                { "TotalApplications", totalApplications },
                { "Pending", pendingApplications },
                { "Approved", approvedApplications },
                { "Rejected", rejectedApplications },
                { "TotalOfficers", totalOfficers }
            };
        }
    }
}