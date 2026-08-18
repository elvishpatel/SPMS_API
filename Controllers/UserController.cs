using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPMS_API.Common;
using SPMS_API.Data;
using SPMS_API.DTOs;
using SPMS_API.Models;

namespace SPMS_API.Controllers
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
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _context.User
                    .Include(u => u.UserType)
                    .Select(u => new ReadUser
                    {
                        UserId = u.UserId,
                        UserTypeId = u.UserTypeId,
                        UserTypeName = u.UserType != null ? u.UserType.UserTypeName : null,
                        FullName = u.FullName,
                        UserCode = u.UserCode,
                        Email = u.Email,
                        MobileNumber = u.MobileNumber,
                        ProfilePicturePath = u.ProfilePicturePath,
                        IsActive = u.IsActive,
                        IsDeleted = u.IsDeleted
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<List<ReadUser>>
                {
                    Success = true,
                    Message = "Users Retrieved Successfully",
                    Data = users
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<ReadUser>>
                {
                    Success = false,
                    Message = "Error occurred while retrieving users",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var user = await _context.User
                    .Include(u => u.UserType)
                    .Where(u => u.UserId == id)
                    .Select(u => new ReadUser
                    {
                        UserId = u.UserId,
                        UserTypeId = u.UserTypeId,
                        UserTypeName = u.UserType != null ? u.UserType.UserTypeName : null,
                        FullName = u.FullName,
                        UserCode = u.UserCode,
                        Email = u.Email,
                        MobileNumber = u.MobileNumber,
                        ProfilePicturePath = u.ProfilePicturePath,
                        IsActive = u.IsActive,
                        IsDeleted = u.IsDeleted
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound(new ApiResponse<ReadUser>
                    {
                        Success = false,
                        Message = "User Not Found",
                        Errors = new List<string> { $"No user found with Id {id}" }
                    });
                }

                return Ok(new ApiResponse<ReadUser>
                {
                    Success = true,
                    Message = "User Retrieved Successfully",
                    Data = user
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadUser>
                {
                    Success = false,
                    Message = "Error occurred while retrieving user",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(CreateUser dto)
        {
            try
            {
                var user = new User
                {
                    UserTypeId = dto.UserTypeId,
                    FullName = dto.FullName,
                    UserCode = dto.UserCode,
                    Email = dto.Email,
                    Password = dto.Password,
                    MobileNumber = dto.MobileNumber,
                    ProfilePicturePath = dto.ProfilePicturePath,
                    IsActive = dto.IsActive
                };

                _context.User.Add(user);
                await _context.SaveChangesAsync();

                var dbUser = await _context.User
                    .Include(u => u.UserType)
                    .FirstOrDefaultAsync(u => u.UserId == user.UserId);

                var response = new ReadUser
                {
                    UserId = user.UserId,
                    UserTypeId = user.UserTypeId,
                    UserTypeName = dbUser?.UserType?.UserTypeName,
                    FullName = user.FullName,
                    UserCode = user.UserCode,
                    Email = user.Email,
                    MobileNumber = user.MobileNumber,
                    ProfilePicturePath = user.ProfilePicturePath,
                    IsActive = user.IsActive,
                    IsDeleted = user.IsDeleted
                };

                return CreatedAtAction(nameof(GetById), new { id = user.UserId }, new ApiResponse<ReadUser>
                {
                    Success = true,
                    Message = "User Added Successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadUser>
                {
                    Success = false,
                    Message = "Error occurred while adding user",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<IActionResult> Update(int id, UpdateUser dto)
        {
            try
            {
                var user = await _context.User.FindAsync(id);

                if (user == null)
                {
                    return NotFound(new ApiResponse<ReadUser>
                    {
                        Success = false,
                        Message = "User Not Found",
                        Errors = new List<string> { $"No user found with Id {id}" }
                    });
                }

                user.UserTypeId = dto.UserTypeId;
                user.FullName = dto.FullName;
                user.UserCode = dto.UserCode;
                user.Email = dto.Email;
                user.MobileNumber = dto.MobileNumber;
                user.ProfilePicturePath = dto.ProfilePicturePath;
                user.IsActive = dto.IsActive;

                await _context.SaveChangesAsync();

                var dbUser = await _context.User
                    .Include(u => u.UserType)
                    .FirstOrDefaultAsync(u => u.UserId == user.UserId);

                var response = new ReadUser
                {
                    UserId = user.UserId,
                    UserTypeId = user.UserTypeId,
                    UserTypeName = dbUser?.UserType?.UserTypeName,
                    FullName = user.FullName,
                    UserCode = user.UserCode,
                    Email = user.Email,
                    MobileNumber = user.MobileNumber,
                    ProfilePicturePath = user.ProfilePicturePath,
                    IsActive = user.IsActive,
                    IsDeleted = user.IsDeleted
                };

                return Ok(new ApiResponse<ReadUser>
                {
                    Success = true,
                    Message = "User Updated Successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadUser>
                {
                    Success = false,
                    Message = "Error occurred while updating user",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = await _context.User.FindAsync(id);

                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User Not Found",
                        Errors = new List<string> { $"No user found with Id {id}" }
                    });
                }

                _context.User.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "User Deleted Successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while deleting user",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}