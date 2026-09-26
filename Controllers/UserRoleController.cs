using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SPMS_API.Common;
using SPMS_API.Data;
using SPMS_API.DTOs;
using SPMS_API.Models;

namespace SPMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IValidator<CreateUserRole> _createValidator;
        private readonly IValidator<UpdateUserRole> _updateValidator;

        public UserRoleController(AppDbContext context, IValidator<CreateUserRole> createValidator, IValidator<UpdateUserRole> updateValidator)
        {
            _context = context;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var userRoles = await _context.UserRole
                    .Include(ur => ur.Role)
                    .Include(ur => ur.User)
                    .Select(ur => new ReadUserRole
                    {
                        RolePermissionId = ur.RolePermissionId,
                        RoleId = ur.RoleId,
                        RoleName = ur.Role != null ? ur.Role.RoleName : null,
                        UserId = ur.UserId,
                        FullName = ur.User != null ? ur.User.FullName : null
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<List<ReadUserRole>>
                {
                    Success = true,
                    Message = "User Roles Retrieved Successfully",
                    Data = userRoles
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<ReadUserRole>>
                {
                    Success = false,
                    Message = "Error occurred while retrieving user roles",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var userRole = await _context.UserRole
                    .Include(ur => ur.Role)
                    .Include(ur => ur.User)
                    .Where(ur => ur.RolePermissionId == id)
                    .Select(ur => new ReadUserRole
                    {
                        RolePermissionId = ur.RolePermissionId,
                        RoleId = ur.RoleId,
                        RoleName = ur.Role != null ? ur.Role.RoleName : null,
                        UserId = ur.UserId,
                        FullName = ur.User != null ? ur.User.FullName : null
                    })
                    .FirstOrDefaultAsync();

                if (userRole == null)
                {
                    return NotFound(new ApiResponse<ReadUserRole>
                    {
                        Success = false,
                        Message = "User Role Not Found",
                        Errors = new List<string> { $"No user role found with Id {id}" }
                    });
                }

                return Ok(new ApiResponse<ReadUserRole>
                {
                    Success = true,
                    Message = "User Role Retrieved Successfully",
                    Data = userRole
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadUserRole>
                {
                    Success = false,
                    Message = "Error occurred while retrieving user role",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(CreateUserRole dto)
        {
            var result = await _createValidator.ValidateAsync(dto);

            if (!result.IsValid)
            {
                return BadRequest(new ApiResponse<ReadUserRole>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = result.Errors.Select(e => e.ErrorMessage).ToList()
                });
            }

            try
            {
                var userRole = new UserRole
                {
                    RoleId = dto.RoleId,
                    UserId = dto.UserId
                };

                _context.UserRole.Add(userRole);
                await _context.SaveChangesAsync();

                var dbUserRole = await _context.UserRole
                    .Include(ur => ur.Role)
                    .Include(ur => ur.User)
                    .FirstOrDefaultAsync(ur => ur.RolePermissionId == userRole.RolePermissionId);

                var response = new ReadUserRole
                {
                    RolePermissionId = userRole.RolePermissionId,
                    RoleId = userRole.RoleId,
                    RoleName = dbUserRole?.Role?.RoleName,
                    UserId = userRole.UserId,
                    FullName = dbUserRole?.User?.FullName
                };

                return CreatedAtAction(nameof(GetById), new { id = userRole.RolePermissionId }, new ApiResponse<ReadUserRole>
                {
                    Success = true,
                    Message = "User Role Added Successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadUserRole>
                {
                    Success = false,
                    Message = "Error occurred while adding user role",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<IActionResult> Update(int id, UpdateUserRole dto)
        {
            var result = await _updateValidator.ValidateAsync(dto);

            if (!result.IsValid)
            {
                return BadRequest(new ApiResponse<ReadUserRole>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = result.Errors.Select(e => e.ErrorMessage).ToList()
                });
            }

            try
            {
                var userRole = await _context.UserRole.FindAsync(id);

                if (userRole == null)
                {
                    return NotFound(new ApiResponse<ReadUserRole>
                    {
                        Success = false,
                        Message = "User Role Not Found",
                        Errors = new List<string> { $"No user role found with Id {id}" }
                    });
                }

                userRole.RoleId = dto.RoleId;
                userRole.UserId = dto.UserId;

                await _context.SaveChangesAsync();

                var dbUserRole = await _context.UserRole
                    .Include(ur => ur.Role)
                    .Include(ur => ur.User)
                    .FirstOrDefaultAsync(ur => ur.RolePermissionId == userRole.RolePermissionId);

                var response = new ReadUserRole
                {
                    RolePermissionId = userRole.RolePermissionId,
                    RoleId = userRole.RoleId,
                    RoleName = dbUserRole?.Role?.RoleName,
                    UserId = userRole.UserId,
                    FullName = dbUserRole?.User?.FullName
                };

                return Ok(new ApiResponse<ReadUserRole>
                {
                    Success = true,
                    Message = "User Role Updated Successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadUserRole>
                {
                    Success = false,
                    Message = "Error occurred while updating user role",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userRole = await _context.UserRole.FindAsync(id);

                if (userRole == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User Role Not Found",
                        Errors = new List<string> { $"No user role found with Id {id}" }
                    });
                }

                _context.UserRole.Remove(userRole);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "User Role Deleted Successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while deleting user role",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("FillDDLUser")]
        public List<SelectListItem> FillDDLUser() {
            var users = _context.User.Select(u => new SelectListItem
            {
                Value = u.UserId.ToString(),
                Text = u.FullName
            }).ToList();
            if(users.Count == 0)
            {
                users.Add(new SelectListItem { Value = "", Text = "No Users Available" });
            }
            return users;
        }

        [HttpGet("FillDDLRole")]
        public List<SelectListItem> FillDDLRole()
        {
            var roles = _context.Role.Select(r => new SelectListItem
            {
                Value = r.RoleId.ToString(),
                Text = r.RoleName
            }).ToList();
            if (roles.Count == 0)
            {
                roles.Add(new SelectListItem { Value = "", Text = "No Roles Available" });
            }
            return roles;
        }
    }
}