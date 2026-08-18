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
            try
            {
                var roles = await _context.Role
                    .Select(r => new ReadRole
                    {
                        RoleId = r.RoleId,
                        RoleName = r.RoleName,
                        Description = r.Description
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<List<ReadRole>>
                {
                    Success = true,
                    Message = "Roles Retrieved Successfully",
                    Data = roles
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<ReadRole>>
                {
                    Success = false,
                    Message = "Error occurred while retrieving roles",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var role = await _context.Role
                    .Where(r => r.RoleId == id)
                    .Select(r => new ReadRole
                    {
                        RoleId = r.RoleId,
                        RoleName = r.RoleName,
                        Description = r.Description
                    })
                    .FirstOrDefaultAsync();

                if (role == null)
                {
                    return NotFound(new ApiResponse<ReadRole>
                    {
                        Success = false,
                        Message = "Role Not Found",
                        Errors = new List<string> { $"No role found with Id {id}" }
                    });
                }

                return Ok(new ApiResponse<ReadRole>
                {
                    Success = true,
                    Message = "Role Retrieved Successfully",
                    Data = role
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadRole>
                {
                    Success = false,
                    Message = "Error occurred while retrieving role",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(CreateRole dto)
        {
            try
            {
                var role = new Role
                {
                    RoleName = dto.RoleName,
                    Description = dto.Description
                };

                _context.Role.Add(role);
                await _context.SaveChangesAsync();

                var response = new ReadRole
                {
                    RoleId = role.RoleId,
                    RoleName = role.RoleName,
                    Description = role.Description
                };

                return CreatedAtAction(nameof(GetById), new { id = role.RoleId }, new ApiResponse<ReadRole>
                {
                    Success = true,
                    Message = "Role Added Successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadRole>
                {
                    Success = false,
                    Message = "Error occurred while adding role",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<IActionResult> Update(int id, UpdateRole dto)
        {
            try
            {
                var role = await _context.Role.FindAsync(id);

                if (role == null)
                {
                    return NotFound(new ApiResponse<ReadRole>
                    {
                        Success = false,
                        Message = "Role Not Found",
                        Errors = new List<string> { $"No role found with Id {id}" }
                    });
                }

                role.RoleName = dto.RoleName;
                role.Description = dto.Description;

                await _context.SaveChangesAsync();

                var response = new ReadRole
                {
                    RoleId = role.RoleId,
                    RoleName = role.RoleName,
                    Description = role.Description
                };

                return Ok(new ApiResponse<ReadRole>
                {
                    Success = true,
                    Message = "Role Updated Successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadRole>
                {
                    Success = false,
                    Message = "Error occurred while updating role",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var role = await _context.Role.FindAsync(id);

                if (role == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Role Not Found",
                        Errors = new List<string> { $"No role found with Id {id}" }
                    });
                }

                _context.Role.Remove(role);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Role Deleted Successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while deleting role",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}