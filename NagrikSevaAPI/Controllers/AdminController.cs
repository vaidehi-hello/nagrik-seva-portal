using Microsoft.AspNetCore.Mvc;
using NagrikSevaAPI.Models;
using NagrikSevaAPI.Repositories.Interfaces;

namespace NagrikSevaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminRepo;
        private readonly IApplicationRepository _appRepo;

        public AdminController(
            IAdminRepository adminRepo,
            IApplicationRepository appRepo)
        {
            _adminRepo = adminRepo;
            _appRepo = appRepo;
        }

        // GET: api/admin/dashboard-stats
        [HttpGet("dashboard-stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _adminRepo.GetDashboardStatsAsync();
            var appStats = await _appRepo.GetStatusCountsAsync();
            return Ok(new { userStats = stats, appStats = appStats });
        }

        // GET: api/admin/users
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminRepo.GetAllUsersAsync();
            var safeUsers = users.Select(u => new {
                u.Id, u.Name, u.Email, u.Phone,
                u.IsEmailVerified, u.CreatedAt,
                roles = u.UserRoles.Select(ur => ur.Role?.Name)
            });
            return Ok(safeUsers);
        }

        // GET: api/admin/officers
        [HttpGet("officers")]
        public async Task<IActionResult> GetOfficers()
        {
            var officers = await _adminRepo
                .GetUsersByRoleAsync("Officer");
            return Ok(officers.Select(u => new {
                u.Id, u.Name, u.Email, u.Phone
            }));
        }

        // DELETE: api/admin/user/1
        [HttpDelete("user/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _adminRepo.DeleteUserAsync(id);
            if (!result)
                return NotFound(new { message = "User not found!" });
            return Ok(new { message = "User deleted!" });
        }

        // POST: api/admin/announcement
        [HttpPost("announcement")]
        public async Task<IActionResult> CreateAnnouncement(
            [FromBody] Announcement announcement)
        {
            var created = await _adminRepo
                .CreateAnnouncementAsync(announcement);
            return Ok(created);
        }

        // GET: api/admin/announcements
        [HttpGet("announcements")]
        public async Task<IActionResult> GetAnnouncements()
        {
            var announcements = await _adminRepo.GetAnnouncementsAsync();
            return Ok(announcements);
        }

        // DELETE: api/admin/announcement/1
        [HttpDelete("announcement/{id}")]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            var result = await _adminRepo.DeleteAnnouncementAsync(id);
            if (!result)
                return NotFound(new { message = "Not found!" });
            return Ok(new { message = "Announcement removed!" });
        }

        // GET: api/admin/all-applications
        [HttpGet("all-applications")]
        public async Task<IActionResult> GetAllApplications()
        {
            var apps = await _appRepo.GetAllAsync();
            return Ok(apps);
        }
    }
}