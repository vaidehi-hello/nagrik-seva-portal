using NagrikSevaAPI.Models;

namespace NagrikSevaAPI.Repositories.Interfaces
{
    public interface IServiceRepository
    {
        Task<List<Service>> GetAllAsync();
        Task<List<Department>> GetAllDepartmentsAsync();
        Task<Service?> GetByIdAsync(int id);
    }
}