using Microsoft.AspNetCore.Mvc;
using SPMS_API.Data;
using SPMS_API.Models;
using Microsoft.EntityFrameworkCore;

namespace SPMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTypeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserTypeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userTypes = await _context.UserType.ToListAsync();
            return Ok(userTypes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userType = await _context.UserType.FindAsync(id);

            if (userType == null)
            {
                return NotFound();
            }

            return Ok(userType);
        }

        [HttpPost]
        public async Task<IActionResult> Add(UserType userType)
        {
            userType.UserTypeId = 0;
            _context.UserType.Add(userType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = userType.UserTypeId }, userType);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UserType userType)
        {
            if (userType.UserTypeId != 0 && userType.UserTypeId != id)
            {
                return BadRequest("ID in route parameter does not match ID in request body.");
            }

            var existingUserType = await _context.UserType.FindAsync(id);

            if (existingUserType == null)
            {
                return NotFound();
            }

            existingUserType.UserTypeName = userType.UserTypeName;
            existingUserType.Description = userType.Description;

            await _context.SaveChangesAsync();

            return Ok(existingUserType);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userType = await _context.UserType.FindAsync(id);

            if (userType == null)
            {
                return NotFound();
            }

            _context.UserType.Remove(userType);
            await _context.SaveChangesAsync();

            return Ok("User Type deleted successfully.");
        }
    }
}