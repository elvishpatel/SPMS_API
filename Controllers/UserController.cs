using FluentValidation;
using JWTDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SPMS_API.Common;
using SPMS_API.Data;
using SPMS_API.DTOs;
using SPMS_API.Models;
using SPMS_API.Services;

namespace SPMS_API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly AppDbContext _context;
        private readonly IValidator<CreateUser> _createValidator;
        private readonly IValidator<UpdateUser> _updateValidator;

        private readonly IFileService _fileService;

        public UserController(AppDbContext context, IValidator<CreateUser> createValidator, IValidator<UpdateUser> updateValidator, TokenService tokenService, IFileService fileService)
        {
            _context = context;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _tokenService = tokenService;
            _fileService = fileService;
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

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            try
            {
                var user = await _context.User.Include(u => u.UserType)
                    .SingleOrDefaultAsync(u => u.Email == dto.Email && u.Password == dto.Password);

                if (user == null)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid Email or password",
                        Errors = new List<string> { "Invalid credentials" }
                    });
                }

                if (user.IsActive != true)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Account is inactive",
                        Errors = new List<string> { "Please contact administrator" }
                    });
                }

                var token = _tokenService.GenerateToken(user);

                var userInfo = new
                {
                    user.UserId,
                    user.FullName,
                    user.Email,
                    user.UserCode,
                    user.MobileNumber,
                    user.ProfilePicturePath,
                    UserType = user.UserType?.UserTypeName,
                    user.UserTypeId
                };

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Login Successful",
                    Data = new { Token = token, User = userInfo }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred during login",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            try
            {
                var studentUserType = await _context.UserType
                    .FirstOrDefaultAsync(ut => ut.UserTypeName == "Student");

                if (studentUserType == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Registration failed",
                        Errors = new List<string> { "Student user type not configured. Please contact administrator." }
                    });
                }

                var existingUser = await _context.User
                    .AnyAsync(u => u.Email == dto.Email);

                if (existingUser)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Registration failed",
                        Errors = new List<string> { "Email already exists" }
                    });
                }

                if (string.IsNullOrEmpty(dto.Password) || dto.Password.Length < 6)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = new List<string> { "Password must be at least 6 characters" }
                    });
                }

                if (dto.Password != dto.ConfirmPassword)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = new List<string> { "Passwords do not match" }
                    });
                }

                if (string.IsNullOrEmpty(dto.FullName))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = new List<string> { "Full name is required" }
                    });
                }

                var user = new User
                {
                    UserTypeId = studentUserType.UserTypeId,
                    UserType = studentUserType,
                    FullName = dto.FullName,
                    UserCode = dto.UserCode ?? string.Empty,
                    Email = dto.Email,
                    Password = dto.Password,
                    MobileNumber = dto.MobileNumber ?? string.Empty,
                    ProfilePicturePath = string.Empty,
                    IsActive = true,
                    IsDeleted = false
                };

                _context.User.Add(user);
                await _context.SaveChangesAsync();

                var token = _tokenService.GenerateToken(user);

                var userInfo = new
                {
                    user.UserId,
                    user.FullName,
                    user.Email,
                    user.UserCode,
                    user.MobileNumber,
                    UserType = studentUserType.UserTypeName,
                    user.UserTypeId
                };

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Registration Successful",
                    Data = new { Token = token, User = userInfo }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred during registration",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Add([FromForm] CreateUser dto)
        {
            var result = await _createValidator.ValidateAsync(dto);

            if (!result.IsValid)
            {
                return BadRequest(new ApiResponse<ReadUser>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = result.Errors.Select(e => e.ErrorMessage).ToList()
                });
            }
            try
            {
                string? uploadedPath = null;
                if (dto.DocumentFile != null && dto.DocumentFile.Length > 0)
                {
                    uploadedPath = await _fileService.UploadFileAsync(dto.DocumentFile, "Users");
                }

                var user = new User
                {
                    UserTypeId = dto.UserTypeId,
                    FullName = dto.FullName,
                    UserCode = dto.UserCode,
                    Email = dto.Email,
                    Password = dto.Password,
                    MobileNumber = dto.MobileNumber,
                    ProfilePicturePath = uploadedPath,
                    IsActive = dto.IsActive,
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
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateUser dto)
        {
            var result = await _updateValidator.ValidateAsync(dto);

            if (!result.IsValid)
            {
                return BadRequest(new ApiResponse<ReadUser>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = result.Errors.Select(e => e.ErrorMessage).ToList()
                });
            }

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

                // Replace the document only when a new file was actually uploaded
                if (dto.DocumentFile != null && dto.DocumentFile.Length > 0)
                {
                    _fileService.DeleteFile(user.ProfilePicturePath);
                    user.ProfilePicturePath = await _fileService.UploadFileAsync(dto.DocumentFile, "Users");
                }

                user.UserTypeId = dto.UserTypeId;
                user.FullName = dto.FullName;
                user.UserCode = dto.UserCode;
                user.Email = dto.Email;
                user.MobileNumber = dto.MobileNumber;
                user.IsActive = dto.IsActive;

                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    user.Password = dto.Password;
                }

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
        public async Task<IActionResult> Delete(int id, [FromQuery] bool deleteFileOnly = false)
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

                // deleteFileOnly=true removes just the uploaded document, keeping the user record
                if (deleteFileOnly)
                {
                    if (string.IsNullOrEmpty(user.ProfilePicturePath))
                    {
                        return BadRequest(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "No document exists for this user.",
                            Errors = new List<string> { "This user has no uploaded document." }
                        });
                    }

                    _fileService.DeleteFile(user.ProfilePicturePath);
                    user.ProfilePicturePath = string.Empty;
                    await _context.SaveChangesAsync();

                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message = "Document deleted successfully.",
                        Data = null
                    });
                }

                // Default: delete the physical file together with the database record
                _fileService.DeleteFile(user.ProfilePicturePath);
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

        [AllowAnonymous]
        [HttpGet("FillDDLUserType")]
        public List<SelectListItem> FillDDLUserType()
        {
            var userTypes = _context.UserType.Select(ut => new SelectListItem
            {
                Value = ut.UserTypeId.ToString(),
                Text = ut.UserTypeName
            }).ToList();
            if (userTypes.Count == 0)
            {
                userTypes.Add(new SelectListItem { Value = "", Text = "No User Types Available" });
            }
            return userTypes;
        }
    }
}