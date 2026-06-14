using Microsoft.EntityFrameworkCore;
using NagrikSevaAPI.Data;
using NagrikSevaAPI.Models;
using NagrikSevaAPI.Repositories.Interfaces;

namespace NagrikSevaAPI.Repositories.Implementations
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly AppDbContext _context;

        public ServiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Service>> GetAllAsync()
        {
            return await _context.Services
                .Include(s => s.Department)
                .Where(s => s.IsActive)
                .ToListAsync();
        }

        public async Task<List<Department>> GetAllDepartmentsAsync()
        {
            return await _context.Departments
                .Where(d => d.IsActive)
                .ToListAsync();
        }

        public async Task<Service?> GetByIdAsync(int id)
        {
            return await _context.Services
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}