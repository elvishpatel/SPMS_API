using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SPMS_API.Common;
using SPMS_API.DTOs;
using SPMS_API.Services;

namespace SPMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IValidator<CreateRole> _createValidator;
        private readonly IValidator<UpdateRole> _updateValidator;

        public RoleController(IRoleService roleService, IValidator<CreateRole> createValidator, IValidator<UpdateRole> updateValidator)
        {
            _roleService = roleService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var roles = await _roleService.GetAllAsync();

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
        [HttpGet("GetById/{id:int:min(1)}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var role = await _roleService.GetByIdAsync(id);

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
        [HttpPost("Add")]
        public async Task<IActionResult> Add(CreateRole dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            try
            {
                var message = await _roleService.AddAsync(dto);
                return Ok(new { Message = message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while adding role",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{id:int:min(1)}")]
        [HttpPut("Update/{id:int:min(1)}")]
        public async Task<IActionResult> Update(int id, UpdateRole dto)
        {
            var result = await _updateValidator.ValidateAsync(dto);

            if (!result.IsValid)
            {
                return BadRequest(new ApiResponse<ReadRole>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = result.Errors.Select(e => e.ErrorMessage).ToList()
                });
            }

            try
            {
                var updated = await _roleService.UpdateAsync(id, dto);

                if (!updated)
                {
                    return NotFound(new ApiResponse<ReadRole>
                    {
                        Success = false,
                        Message = "Role Not Found",
                        Errors = new List<string> { $"No role found with Id {id}" }
                    });
                }

                var response = await _roleService.GetByIdAsync(id);

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
        [HttpDelete("Delete/{id:int:min(1)}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _roleService.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Role Not Found",
                        Errors = new List<string> { $"No role found with Id {id}" }
                    });
                }

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