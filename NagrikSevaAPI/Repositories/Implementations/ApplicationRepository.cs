using Microsoft.EntityFrameworkCore;
using NagrikSevaAPI.Data;
using NagrikSevaAPI.Models;
using NagrikSevaAPI.Repositories.Interfaces;

namespace NagrikSevaAPI.Repositories.Implementations
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly AppDbContext _context;

        public ApplicationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Application> CreateAsync(Application application)
        {
            // Generate unique application number
            application.AppNo = "NS" + DateTime.Now.ToString("yyyyMMddHHmmss");
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<List<Application>> GetByCitizenAsync(int citizenId)
        {
            return await _context.Applications
                .Include(a => a.Service)
                    .ThenInclude(s => s.Department)
                .Where(a => a.CitizenId == citizenId)
                .OrderByDescending(a => a.SubmittedOn)
                .ToListAsync();
        }

        public async Task<List<Application>> GetAllAsync()
        {
            return await _context.Applications
                .Include(a => a.Citizen)
                .Include(a => a.Service)
                    .ThenInclude(s => s.Department)
                 .Include(a => a.Details) // ✅ important   
                .OrderByDescending(a => a.SubmittedOn)
                .ToListAsync();
        }

        public async Task<Application?> GetByIdAsync(int id)
        {
            return await _context.Applications
                .Include(a => a.Citizen)
                .Include(a => a.Service)
                .Include(a => a.Trackings)
                .Include(a => a.Details) // ✅ add this

                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Application?> GetByAppNoAsync(string appNo)
        {
            return await _context.Applications
                .Include(a => a.Service)
                .Include(a => a.Trackings)
                .FirstOrDefaultAsync(a => a.AppNo == appNo);
        }

        public async Task<Application?> UpdateStatusAsync(
            int id, string status, string? remarks, int officerId)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null) return null;

            application.Status = status;
            application.UpdatedOn = DateTime.Now;

            // Add tracking record
            var tracking = new ApplicationTracking
            {
                ApplicationId = id,
                OfficerId = officerId,
                Status = status,
                Remarks = remarks,
                ActionDate = DateTime.Now
            };

            _context.ApplicationTrackings.Add(tracking);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<List<Application>> GetByOfficerAsync(int officerId)
        {
            return await _context.Applications
                .Include(a => a.Citizen)
                .Include(a => a.Service)
                .Where(a => a.Status == "Pending" ||
                            a.Status == "UnderReview")
                .OrderByDescending(a => a.SubmittedOn)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetStatusCountsAsync()
        {
            var counts = await _context.Applications
                .GroupBy(a => a.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            return counts.ToDictionary(x => x.Status, x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetCitizenStatusCountsAsync(
            int citizenId)
        {
            var counts = await _context.Applications
                .Where(a => a.CitizenId == citizenId)
                .GroupBy(a => a.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            return counts.ToDictionary(x => x.Status, x => x.Count);
        }

        // ✅ Add these two methods inside ApplicationRepository class

public async Task SaveDetailsAsync(ApplicationDetails details)
{
    _context.ApplicationDetails.Add(details);
    await _context.SaveChangesAsync();
}

public async Task SaveCertificatePathAsync(
    int applicationId, string certFileName)
{
    var app = await _context.Applications
        .FindAsync(applicationId);
    if (app != null)
    {
        app.CertificatePath = certFileName;
        app.UpdatedOn = DateTime.Now;
        await _context.SaveChangesAsync();
    }
}
    }
}