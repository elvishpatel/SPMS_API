using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SPMS_API.Common;
using SPMS_API.Data;
using SPMS_API.DTOs;
using SPMS_API.Models;
using System.Security.Claims;

namespace SPMS_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectAllocationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IValidator<CreateProjectAllocation> _createValidator;
        private readonly IValidator<UpdateProjectAllocation> _updateValidator;

        public ProjectAllocationController(AppDbContext context, IValidator<CreateProjectAllocation> createValidator, IValidator<UpdateProjectAllocation> updateValidator)
        {
            _context = context;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        // Reads the logged-in user's id from the JWT (UserId, sub, or NameIdentifier claim)
        private int GetLoggedInUserId()
        {
            var claim = User.Claims.FirstOrDefault(c => (c.Type == ClaimTypes.NameIdentifier || c.Type == "UserId" || c.Type == "sub" || c.Type == "id") && int.TryParse(c.Value, out _));
            return claim != null && int.TryParse(claim.Value, out var parsed) ? parsed : 0;
        }

        private bool IsAdmin() => User.IsInRole("Admin") || User.HasClaim(ClaimTypes.Role, "Admin") || User.HasClaim("role", "Admin");
        private bool IsFaculty() => User.IsInRole("Faculty") || User.HasClaim(ClaimTypes.Role, "Faculty") || User.HasClaim("role", "Faculty");

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var userId = GetLoggedInUserId();
                var isAdmin = IsAdmin();
                var isFaculty = IsFaculty();

                var query = _context.ProjectAllocation
                    .Include(p => p.ProjectMaster)
                    .Include(p => p.Student)
                    .Include(p => p.Faculty)
                    .AsQueryable();

                // Admin sees everything; faculty only their own allocations; students only theirs.
                if (!isAdmin && isFaculty)
                {
                    query = query.Where(p => p.FacultyID == userId);
                }
                else if (!isAdmin)
                {
                    query = query.Where(p => p.StudentID == userId);
                }

                var projectAllocations = await query
                    .Select(p => new ReadProjectAllocation
                    {
                        ProjectAllocationID = p.ProjectAllocationID,
                        ProjectID = p.ProjectID,
                        ProjectTitle = p.ProjectMaster != null ? p.ProjectMaster.ProjectTitle : null,
                        ProjectDescription = p.ProjectMaster != null ? p.ProjectMaster.Description : null,
                        StudentID = p.StudentID,
                        StudentName = p.Student != null ? p.Student.FullName : null,
                        FacultyID = p.FacultyID,
                        FacultyName = p.Faculty != null ? p.Faculty.FullName : null,
                        AssignedDate = p.AssignedDate,
                        ProjectStartDate = p.ProjectStartDate,
                        ProjectEndDate = p.ProjectEndDate,
                        TotalTasksGiven = p.TotalTasksGiven,
                        TotalCompletedTasks = p.TotalCompletedTasks,
                        ProgressPercentage = p.ProgressPercentage,
                        OverAllGrade = p.OverAllGrade
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<List<ReadProjectAllocation>>
                {
                    Success = true,
                    Message = "Project Allocations Retrieved Successfully",
                    Data = projectAllocations
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<ReadProjectAllocation>>
                {
                    Success = false,
                    Message = "Error occurred while retrieving project allocations",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var userId = GetLoggedInUserId();
                var isAdmin = IsAdmin();
                var isFaculty = IsFaculty();

                var query = _context.ProjectAllocation
                    .Include(p => p.ProjectMaster)
                    .Include(p => p.Student)
                    .Include(p => p.Faculty)
                    .Where(p => p.ProjectAllocationID == id)
                    .AsQueryable();

                // Non-admin users can only see their own allocation
                if (!isAdmin && isFaculty)
                {
                    query = query.Where(p => p.FacultyID == userId);
                }
                else if (!isAdmin)
                {
                    query = query.Where(p => p.StudentID == userId);
                }

                var projectAllocation = await query
                    .Select(p => new ReadProjectAllocation
                    {
                        ProjectAllocationID = p.ProjectAllocationID,
                        ProjectID = p.ProjectID,
                        ProjectTitle = p.ProjectMaster != null ? p.ProjectMaster.ProjectTitle : null,
                        ProjectDescription = p.ProjectMaster != null ? p.ProjectMaster.Description : null,
                        StudentID = p.StudentID,
                        StudentName = p.Student != null ? p.Student.FullName : null,
                        FacultyID = p.FacultyID,
                        FacultyName = p.Faculty != null ? p.Faculty.FullName : null,
                        AssignedDate = p.AssignedDate,
                        ProjectStartDate = p.ProjectStartDate,
                        ProjectEndDate = p.ProjectEndDate,
                        TotalTasksGiven = p.TotalTasksGiven,
                        TotalCompletedTasks = p.TotalCompletedTasks,
                        ProgressPercentage = p.ProgressPercentage,
                        OverAllGrade = p.OverAllGrade
                    })
                    .FirstOrDefaultAsync();

                if (projectAllocation == null)
                {
                    return NotFound(new ApiResponse<ReadProjectAllocation>
                    {
                        Success = false,
                        Message = "Project Allocation Not Found",
                        Errors = new List<string> { $"No project allocation found with Id {id}" }
                    });
                }

                return Ok(new ApiResponse<ReadProjectAllocation>
                {
                    Success = true,
                    Message = "Project Allocation Retrieved Successfully",
                    Data = projectAllocation
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadProjectAllocation>
                {
                    Success = false,
                    Message = "Error occurred while retrieving project allocation",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Add(CreateProjectAllocation dto)
        {
            var result = await _createValidator.ValidateAsync(dto);

            if (!result.IsValid)
            {
                return BadRequest(new ApiResponse<ReadProjectAllocation>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = result.Errors.Select(e => e.ErrorMessage).ToList()
                });
            }

            try
            {
                var projectAllocation = new ProjectAllocation
                {
                    ProjectID = dto.ProjectID,
                    StudentID = dto.StudentID,
                    FacultyID = dto.FacultyID,
                    AssignedDate = dto.AssignedDate,
                    ProjectStartDate = dto.ProjectStartDate,
                    ProjectEndDate = dto.ProjectEndDate,
                    TotalTasksGiven = dto.TotalTasksGiven,
                    TotalCompletedTasks = dto.TotalCompletedTasks,
                    ProgressPercentage = dto.ProgressPercentage,
                    OverAllGrade = dto.OverAllGrade
                };

                _context.ProjectAllocation.Add(projectAllocation);
                await _context.SaveChangesAsync();

                var dbAlloc = await _context.ProjectAllocation
                    .Include(p => p.ProjectMaster)
                    .Include(p => p.Student)
                    .Include(p => p.Faculty)
                    .FirstOrDefaultAsync(p => p.ProjectAllocationID == projectAllocation.ProjectAllocationID);

                var response = new ReadProjectAllocation
                {
                    ProjectAllocationID = projectAllocation.ProjectAllocationID,
                    ProjectID = projectAllocation.ProjectID,
                    ProjectTitle = dbAlloc?.ProjectMaster?.ProjectTitle,
                    ProjectDescription = dbAlloc?.ProjectMaster?.Description,
                    StudentID = projectAllocation.StudentID,
                    StudentName = dbAlloc?.Student?.FullName,
                    FacultyID = projectAllocation.FacultyID,
                    FacultyName = dbAlloc?.Faculty?.FullName,
                    AssignedDate = projectAllocation.AssignedDate,
                    ProjectStartDate = projectAllocation.ProjectStartDate,
                    ProjectEndDate = projectAllocation.ProjectEndDate,
                    TotalTasksGiven = projectAllocation.TotalTasksGiven,
                    TotalCompletedTasks = projectAllocation.TotalCompletedTasks,
                    ProgressPercentage = projectAllocation.ProgressPercentage,
                    OverAllGrade = projectAllocation.OverAllGrade
                };

                return CreatedAtAction(nameof(GetById), new { id = projectAllocation.ProjectAllocationID }, new ApiResponse<ReadProjectAllocation>
                {
                    Success = true,
                    Message = "Project Allocation Added Successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadProjectAllocation>
                {
                    Success = false,
                    Message = "Error occurred while adding project allocation",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{id:int:min(1)}")]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Update(int id, UpdateProjectAllocation dto)
        {
            var result = await _updateValidator.ValidateAsync(dto);

            if (!result.IsValid)
            {
                return BadRequest(new ApiResponse<ReadProjectAllocation>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = result.Errors.Select(e => e.ErrorMessage).ToList()
                });
            }

            try
            {
                var existingProjectAllocation = await _context.ProjectAllocation.FindAsync(id);

                if (existingProjectAllocation == null)
                {
                    return NotFound(new ApiResponse<ReadProjectAllocation>
                    {
                        Success = false,
                        Message = "Project Allocation Not Found",
                        Errors = new List<string> { $"No project allocation found with Id {id}" }
                    });
                }

                existingProjectAllocation.ProjectID = dto.ProjectID;
                existingProjectAllocation.StudentID = dto.StudentID;
                existingProjectAllocation.FacultyID = dto.FacultyID;
                existingProjectAllocation.AssignedDate = dto.AssignedDate;
                existingProjectAllocation.ProjectStartDate = dto.ProjectStartDate;
                existingProjectAllocation.ProjectEndDate = dto.ProjectEndDate;
                existingProjectAllocation.TotalTasksGiven = dto.TotalTasksGiven;
                existingProjectAllocation.TotalCompletedTasks = dto.TotalCompletedTasks;
                existingProjectAllocation.ProgressPercentage = dto.ProgressPercentage;
                existingProjectAllocation.OverAllGrade = dto.OverAllGrade;

                await _context.SaveChangesAsync();

                var dbAlloc = await _context.ProjectAllocation
                    .Include(p => p.ProjectMaster)
                    .Include(p => p.Student)
                    .Include(p => p.Faculty)
                    .FirstOrDefaultAsync(p => p.ProjectAllocationID == existingProjectAllocation.ProjectAllocationID);

                var response = new ReadProjectAllocation
                {
                    ProjectAllocationID = existingProjectAllocation.ProjectAllocationID,
                    ProjectID = existingProjectAllocation.ProjectID,
                    ProjectTitle = dbAlloc?.ProjectMaster?.ProjectTitle,
                    ProjectDescription = dbAlloc?.ProjectMaster?.Description,
                    StudentID = existingProjectAllocation.StudentID,
                    StudentName = dbAlloc?.Student?.FullName,
                    FacultyID = existingProjectAllocation.FacultyID,
                    FacultyName = dbAlloc?.Faculty?.FullName,
                    AssignedDate = existingProjectAllocation.AssignedDate,
                    ProjectStartDate = existingProjectAllocation.ProjectStartDate,
                    ProjectEndDate = existingProjectAllocation.ProjectEndDate,
                    TotalTasksGiven = existingProjectAllocation.TotalTasksGiven,
                    TotalCompletedTasks = existingProjectAllocation.TotalCompletedTasks,
                    ProgressPercentage = existingProjectAllocation.ProgressPercentage,
                    OverAllGrade = existingProjectAllocation.OverAllGrade
                };

                return Ok(new ApiResponse<ReadProjectAllocation>
                {
                    Success = true,
                    Message = "Project Allocation Updated Successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadProjectAllocation>
                {
                    Success = false,
                    Message = "Error occurred while updating project allocation",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("{id:int:min(1)}")]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var projectAllocation = await _context.ProjectAllocation.FindAsync(id);

                if (projectAllocation == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Project Allocation Not Found",
                        Errors = new List<string> { $"No project allocation found with Id {id}" }
                    });
                }

                _context.ProjectAllocation.Remove(projectAllocation);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Project Allocation Deleted Successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while deleting project allocation",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // Faculty progress update: task totals, progress and grade only
        [HttpPatch("{id:int:min(1)}/progress")]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> UpdateProgress(int id, UpdateProjectProgress dto)
        {
            if (dto.TotalTasksGiven < 0 || dto.TotalCompletedTasks < 0 ||
                dto.TotalCompletedTasks > dto.TotalTasksGiven ||
                dto.ProgressPercentage < 0 || dto.ProgressPercentage > 100)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = new List<string> { "Provide valid task totals and a progress percentage between 0 and 100." }
                });
            }

            var userId = GetLoggedInUserId();
            var isAdmin = IsAdmin();

            var allocation = await _context.ProjectAllocation.FindAsync(id);
            if (allocation == null || (!isAdmin && allocation.FacultyID != userId))
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Project Allocation Not Found",
                    Errors = new List<string> { $"No project allocation found with Id {id}" }
                });
            }

            allocation.TotalTasksGiven = dto.TotalTasksGiven;
            allocation.TotalCompletedTasks = dto.TotalCompletedTasks;
            allocation.ProgressPercentage = dto.ProgressPercentage;
            allocation.OverAllGrade = dto.OverAllGrade;
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Project progress updated successfully",
                Data = null
            });
        }

        [HttpGet("FillDDLProjectMaster")]
        public List<SelectListItem> FillDDLProjectMaster()
        {
            var projects = _context.ProjectMaster.Select(p => new SelectListItem
            {
                Value = p.ProjectId.ToString(),
                Text = p.ProjectTitle
            }).ToList();
            if (projects.Count == 0)
            {
                projects.Add(new SelectListItem { Value = "", Text = "No Projects Available" });
            }
            return projects;
        }

        [HttpGet("FillDDLStudent")]
        public List<SelectListItem> FillDDLStudent()
        {
            var students = _context.User
                .Include(u => u.UserType)
                .Where(u => u.UserType != null && u.UserType.UserTypeName == "Student")
                .Select(u => new SelectListItem
                {
                    Value = u.UserId.ToString(),
                    Text = u.FullName
                }).ToList();
            if (students.Count == 0)
            {
                students.Add(new SelectListItem { Value = "", Text = "No Students Available" });
            }
            return students;
        }

        [HttpGet("FillDDLFaculty")]
        public List<SelectListItem> FillDDLFaculty()
        {
            var faculty = _context.User
                .Include(u => u.UserType)
                .Where(u => u.UserType != null && u.UserType.UserTypeName == "Faculty")
                .Select(u => new SelectListItem
                {
                    Value = u.UserId.ToString(),
                    Text = u.FullName
                }).ToList();
            if (faculty.Count == 0)
            {
                faculty.Add(new SelectListItem { Value = "", Text = "No Faculty Available" });
            }
            return faculty;
        }
    }
}