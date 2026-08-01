using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPMS_API.Data;
using SPMS_API.Models;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace SPMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<User>> GetALl()
        {
            var users = await _context.User.ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        [HttpPost]
        public async Task<ActionResult<User>> Add(User user)
        {
            user.UserId = 0;
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = user.UserId }, user);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Update(int id, User user)
        {
            if (user.UserId != 0 && user.UserId != id)
            {
                return BadRequest("ID in route parameter does not match ID in request body.");
            }

            var existingUser = await _context.User.FindAsync(id);

            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.UserTypeId = user.UserTypeId;
            existingUser.FullName = user.FullName;
            existingUser.UserCode = user.UserCode;
            existingUser.Email = user.Email;
            existingUser.Password = user.Password;
            existingUser.MobileNumber = user.MobileNumber;
            existingUser.ProfilePicturePath = user.ProfilePicturePath;
            existingUser.IsActive = user.IsActive;
            existingUser.IsDeleted = user.IsDeleted;

            await _context.SaveChangesAsync();

            return Ok(existingUser);
        }

        [HttpDelete("{id}")]

        public async Task<ActionResult<SPMS_API.Models.User>> Delete(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
