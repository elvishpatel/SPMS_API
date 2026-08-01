using Microsoft.AspNetCore.Mvc;
using SPMS_API.Data;
using SPMS_API.Models;
using Microsoft.EntityFrameworkCore;

namespace SPMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _context.Role.ToListAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _context.Role.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Role role)
        {
            role.RoleId = 0;
            _context.Role.Add(role);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = role.RoleId }, role);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Role role)
        {
            if (role.RoleId != 0 && role.RoleId != id)
            {
                return BadRequest("ID in route parameter does not match ID in request body.");
            }

            var exists = await _context.Role.FindAsync(id);
            if (exists == null)
            {
                return NotFound();
            }

            exists.RoleName = role.RoleName;
            exists.Description = role.Description;

            await _context.SaveChangesAsync();
            return Ok(exists);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _context.Role.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            _context.Role.Remove(role);
            await _context.SaveChangesAsync();
            return Ok("Role deleted successfully.");
        }
    }
}
