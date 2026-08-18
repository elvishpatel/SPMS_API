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
            try
            {
                var userTypes = await _context.UserType
                    .Select(ut => new ReadUserType
                    {
                        UserTypeId = ut.UserTypeId,
                        UserTypeName = ut.UserTypeName,
                        Description = ut.Description
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<List<ReadUserType>>
                {
                    Success = true,
                    Message = "User Types Retrieved Successfully",
                    Data = userTypes
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<ReadUserType>>
                {
                    Success = false,
                    Message = "Error occurred while retrieving user types",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var userType = await _context.UserType
                    .Where(ut => ut.UserTypeId == id)
                    .Select(ut => new ReadUserType
                    {
                        UserTypeId = ut.UserTypeId,
                        UserTypeName = ut.UserTypeName,
                        Description = ut.Description
                    })
                    .FirstOrDefaultAsync();

                if (userType == null)
                {
                    return NotFound(new ApiResponse<ReadUserType>
                    {
                        Success = false,
                        Message = "User Type Not Found",
                        Errors = new List<string> { $"No user type found with Id {id}" }
                    });
                }

                return Ok(new ApiResponse<ReadUserType>
                {
                    Success = true,
                    Message = "User Type Retrieved Successfully",
                    Data = userType
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadUserType>
                {
                    Success = false,
                    Message = "Error occurred while retrieving user type",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(CreateUserType dto)
        {
            try
            {
                var userType = new UserType
                {
                    UserTypeName = dto.UserTypeName,
                    Description = dto.Description
                };

                _context.UserType.Add(userType);
                await _context.SaveChangesAsync();

                var response = new ReadUserType
                {
                    UserTypeId = userType.UserTypeId,
                    UserTypeName = userType.UserTypeName,
                    Description = userType.Description
                };

                return CreatedAtAction(nameof(GetById), new { id = userType.UserTypeId }, new ApiResponse<ReadUserType>
                {
                    Success = true,
                    Message = "User Type Added Successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadUserType>
                {
                    Success = false,
                    Message = "Error occurred while adding user type",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<IActionResult> Update(int id, UpdateUserType dto)
        {
            try
            {
                var userType = await _context.UserType.FindAsync(id);

                if (userType == null)
                {
                    return NotFound(new ApiResponse<ReadUserType>
                    {
                        Success = false,
                        Message = "User Type Not Found",
                        Errors = new List<string> { $"No user type found with Id {id}" }
                    });
                }

                userType.UserTypeName = dto.UserTypeName;
                userType.Description = dto.Description;

                await _context.SaveChangesAsync();

                var response = new ReadUserType
                {
                    UserTypeId = userType.UserTypeId,
                    UserTypeName = userType.UserTypeName,
                    Description = userType.Description
                };

                return Ok(new ApiResponse<ReadUserType>
                {
                    Success = true,
                    Message = "User Type Updated Successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadUserType>
                {
                    Success = false,
                    Message = "Error occurred while updating user type",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userType = await _context.UserType.FindAsync(id);

                if (userType == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User Type Not Found",
                        Errors = new List<string> { $"No user type found with Id {id}" }
                    });
                }

                _context.UserType.Remove(userType);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "User Type Deleted Successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while deleting user type",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}