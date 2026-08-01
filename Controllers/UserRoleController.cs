using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPMS_API.Data;
using SPMS_API.Models;

namespace SPMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserRoleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<UserRole>> GetALl()
        {
            var userroles = await _context.UserRole.ToListAsync();
            return Ok(userroles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserRole>> GetById(int id)
        {
            var userrole = await _context.UserRole.FindAsync(id);
            if (userrole == null)
            {
                return NotFound();
            }
            return Ok(userrole);
        }
        [HttpPost]
        public async Task<ActionResult<UserRole>> Add(UserRole userrole)
        {
            userrole.RolePermissionId = 0;
            _context.UserRole.Add(userrole);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = userrole.RolePermissionId }, userrole);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<UserRole>> Update(int id, UserRole userrole)
        {

            if (userrole.RolePermissionId != 0 && userrole.RolePermissionId != id)
            {
                return BadRequest("ID in route parameter does not match ID in request body.");
            }

            var existingUserRole = await _context.UserRole.FindAsync(id);

            if (existingUserRole == null)
            {
                return NotFound();
            }

            existingUserRole.RoleId = userrole.RoleId;
            existingUserRole.UserId = userrole.UserId;

            await _context.SaveChangesAsync();

            return Ok(existingUserRole);


        }
        [HttpDelete("{id}")]

        public async Task<ActionResult<SPMS_API.Models.UserRole>> Delete(int id)
        {
            var userrole = await _context.UserRole.FindAsync(id);
            if (userrole == null)
            {
                return NotFound();
            }
            _context.UserRole.Remove(userrole);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
