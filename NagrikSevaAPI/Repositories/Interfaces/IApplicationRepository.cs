using NagrikSevaAPI.Models;

namespace NagrikSevaAPI.Repositories.Interfaces
{
    public interface IApplicationRepository
    {
        Task<Application> CreateAsync(Application application);
        Task<List<Application>> GetByCitizenAsync(int citizenId);
        Task<List<Application>> GetAllAsync();
        Task<Application?> GetByIdAsync(int id);
        Task<Application?> GetByAppNoAsync(string appNo);
        Task<Application?> UpdateStatusAsync(
            int id, string status, string? remarks, int officerId);
        Task<List<Application>> GetByOfficerAsync(int officerId);
        Task<Dictionary<string, int>> GetStatusCountsAsync();
        Task<Dictionary<string, int>> GetCitizenStatusCountsAsync(
            int citizenId);

            // ✅ NEW methods
        Task SaveDetailsAsync(ApplicationDetails details);
        Task SaveCertificatePathAsync(
            int applicationId, string certFileName);
    }
}