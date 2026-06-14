using Microsoft.AspNetCore.Mvc;
using NagrikSevaAPI.Repositories.Interfaces;

namespace NagrikSevaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly IServiceRepository _serviceRepo;
        private readonly IAdminRepository _adminRepo;

        public PublicController(
            IServiceRepository serviceRepo,
            IAdminRepository adminRepo)
        {
            _serviceRepo = serviceRepo;
            _adminRepo = adminRepo;
        }

        // GET: api/public/services
        [HttpGet("services")]
        public async Task<IActionResult> GetServices()
        {
            try
            {
                var services = await _serviceRepo.GetAllAsync();
                return Ok(services);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {
                    message = "Error fetching services: " + ex.Message
                });
            }
        }

        // GET: api/public/departments
        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments()
        {
            try
            {
                var departments = await _serviceRepo
                    .GetAllDepartmentsAsync();
                return Ok(departments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {
                    message = "Error: " + ex.Message
                });
            }
        }

        // GET: api/public/announcements
        [HttpGet("announcements")]
        public async Task<IActionResult> GetAnnouncements()
        {
            try
            {
                var announcements = await _adminRepo
                    .GetAnnouncementsAsync();
                return Ok(announcements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {
                    message = "Error: " + ex.Message
                });
            }
        }
    }
}