using Microsoft.EntityFrameworkCore;
using SPMS_API.Data;
using SPMS_API.Models;
using Task = System.Threading.Tasks.Task;

namespace SPMS_API.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await _context.Role.ToListAsync();
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _context.Role.FindAsync(id);
        }

        public async Task AddAsync(Role role)
        {
            await _context.Role.AddAsync(role);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Role role)
        {
            _context.Role.Update(role);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Role role)
        {
            _context.Role.Remove(role);
            await _context.SaveChangesAsync();
        }
    }
}
