using SPMS_API.DTOs;
using SPMS_API.Models;
using SPMS_API.Repositories;
using Task = System.Threading.Tasks.Task;

namespace SPMS_API.Services
{
    public class RoleService : IRoleService
    {
        private readonly List<Role> _roles = new();

        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<ReadRole>> GetAllAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return roles.Select(r => new ReadRole
            {
                RoleId = r.RoleId,
                RoleName = r.RoleName,
                Description = r.Description
            }).ToList();
        }

        public async Task<ReadRole?> GetByIdAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return null;
            }

            return new ReadRole
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Description = role.Description
            };
        }

        public async Task<string> AddAsync(CreateRole dto)
        {
            var role = new Role
            {
                RoleName = dto.RoleName,
                Description = dto.Description
            };

            await _roleRepository.AddAsync(role);

            return "Record Inserted";
        }

        public async Task<bool> UpdateAsync(int id, UpdateRole dto)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return false;
            }

            role.RoleName = dto.RoleName;
            role.Description = dto.Description;

            await _roleRepository.UpdateAsync(role);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return false;
            }

            await _roleRepository.DeleteAsync(role);
            return true;
        }
    }
}
