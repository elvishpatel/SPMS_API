using SPMS_API.DTOs;

namespace SPMS_API.Services
{
    public interface IRoleService
    {
        Task<List<ReadRole>> GetAllAsync();
        Task<ReadRole?> GetByIdAsync(int id);
        Task<string> AddAsync(CreateRole dto);
        Task<bool> UpdateAsync(int id, UpdateRole dto);
        Task<bool> DeleteAsync(int id);
    }
}
