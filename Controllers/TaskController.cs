using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SPMS_API.Common;
using SPMS_API.Data;
using SPMS_API.DTOs;
using System.Security.Claims;

namespace SPMS_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IValidator<CreateTask> _createValidator;
        private readonly IValidator<UpdateTask> _updateValidator;

        public TaskController(AppDbContext context, IValidator<CreateTask> createValidator, IValidator<UpdateTask> updateValidator)
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

                var query = _context.Task
                    .Include(t => t.ProjectAllocation)
                        .ThenInclude(pa => pa.ProjectMaster)
                    .Include(t => t.ProjectAllocation)
                        .ThenInclude(pa => pa.Student)
                    .Include(t => t.TaskStatus)
                    .Include(t => t.TaskPriority)
                    .AsQueryable();

                // Admin sees everything; faculty only tasks of their own allocations;
                // students only tasks assigned to them.
                if (!isAdmin && isFaculty)
                {
                    query = query.Where(t => t.ProjectAllocation.FacultyID == userId);
                }
                else if (!isAdmin)
                {
                    query = query.Where(t => t.ProjectAllocation.StudentID == userId);
                }

                var tasks = await query
                    .Select(t => new ReadTask
                    {
                        TaskID = t.TaskID,
                        ProjectAllocationID = t.ProjectAllocationID,
                        ProjectTitle = t.ProjectAllocation != null && t.ProjectAllocation.ProjectMaster != null ? t.ProjectAllocation.ProjectMaster.ProjectTitle : null,
                        StudentName = t.ProjectAllocation != null && t.ProjectAllocation.Student != null ? t.ProjectAllocation.Student.FullName : null,
                        TaskTitle = t.TaskTitle,
                        TaskDescription = t.TaskDescription,
                        TaskStatusID = t.TaskStatusID,
                        TaskStatusName = t.TaskStatus != null ? t.TaskStatus.TaskStatusName : null,
                        TaskPriorityID = t.TaskPriorityID,
                        TaskPriorityName = t.TaskPriority != null ? t.TaskPriority.TaskPriorityName : null,
                        AssignedScore = t.AssignedScore,
                        EarnedScore = t.EarnedScore,
                        ProgressPercentage = t.ProgressPercentage,
                        TaskAssignedDate = t.TaskAssignnedDate,
                        TaskStartDate = t.TaskStartDate,
                        TaskDueDate = t.TaskDueDate,
                        TaskEndDate = t.TaskEndDate,
                        TaskCompletedDate = t.TaskCompletedTime,
                        NextFollowUpDate = t.NextFollowUpDate,
                        FacultyRemarks = t.FacultyRemarks,
                        StudentRemarks = t.StudentRemarks
                    }).ToListAsync();

                return Ok(new ApiResponse<List<ReadTask>>
                {
                    Success = true,
                    Message = "Tasks Retrieved Successfully",
                    Data = tasks
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<ReadTask>>
                {
                    Success = false,
                    Message = "Error occurred while retrieving tasks",
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

                var query = _context.Task
                    .Include(t => t.ProjectAllocation)
                        .ThenInclude(pa => pa.ProjectMaster)
                    .Include(t => t.ProjectAllocation)
                        .ThenInclude(pa => pa.Student)
                    .Include(t => t.TaskStatus)
                    .Include(t => t.TaskPriority)
                    .Where(t => t.TaskID == id)
                    .AsQueryable();

                // Non-admin users can only see their own task
                if (!isAdmin && isFaculty)
                {
                    query = query.Where(t => t.ProjectAllocation.FacultyID == userId);
                }
                else if (!isAdmin)
                {
                    query = query.Where(t => t.ProjectAllocation.StudentID == userId);
                }

                var task = await query
                    .Select(t => new ReadTask
                    {
                        TaskID = t.TaskID,
                        ProjectAllocationID = t.ProjectAllocationID,
                        ProjectTitle = t.ProjectAllocation != null && t.ProjectAllocation.ProjectMaster != null ? t.ProjectAllocation.ProjectMaster.ProjectTitle : null,
                        StudentName = t.ProjectAllocation != null && t.ProjectAllocation.Student != null ? t.ProjectAllocation.Student.FullName : null,
                        TaskTitle = t.TaskTitle,
                        TaskDescription = t.TaskDescription,
                        TaskStatusID = t.TaskStatusID,
                        TaskStatusName = t.TaskStatus != null ? t.TaskStatus.TaskStatusName : null,
                        TaskPriorityID = t.TaskPriorityID,
                        TaskPriorityName = t.TaskPriority != null ? t.TaskPriority.TaskPriorityName : null,
                        AssignedScore = t.AssignedScore,
                        EarnedScore = t.EarnedScore,
                        ProgressPercentage = t.ProgressPercentage,
                        TaskAssignedDate = t.TaskAssignnedDate,
                        TaskStartDate = t.TaskStartDate,
                        TaskDueDate = t.TaskDueDate,
                        TaskEndDate = t.TaskEndDate,
                        TaskCompletedDate = t.TaskCompletedTime,
                        NextFollowUpDate = t.NextFollowUpDate,
                        FacultyRemarks = t.FacultyRemarks,
                        StudentRemarks = t.StudentRemarks
                    })
                    .FirstOrDefaultAsync();

                if (task == null)
                {
                    return NotFound(new ApiResponse<ReadTask>
                    {
                        Success = false,
                        Message = "Task Not Found",
                        Errors = new List<string> { $"No task found with Id {id}" }
                    });
                }

                return Ok(new ApiResponse<ReadTask>
                {
                    Success = true,
                    Message = "Task Retrieved Successfully",
                    Data = task
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadTask>
                {
                    Success = false,
                    Message = "Error occurred while retrieving task",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Add(CreateTask dto)
        {
            var result = await _createValidator.ValidateAsync(dto);

            if (!result.IsValid)
            {
                return BadRequest(new ApiResponse<ReadTask>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = result.Errors.Select(e => e.ErrorMessage).ToList()
                });
            }

            try
            {
                var task = new SPMS_API.Models.Task
                {
                    ProjectAllocationID = dto.ProjectAllocationID,
                    TaskTitle = dto.TaskTitle,
                    TaskDescription = dto.TaskDescription,
                    TaskStatusID = dto.TaskStatusID,
                    TaskPriorityID = dto.TaskPriorityID,
                    AssignedScore = dto.AssignedScore,
                    EarnedScore = dto.EarnedScore,
                    ProgressPercentage = dto.ProgressPercentage,
                    TaskAssignnedDate = dto.TaskAssignnedDate,
                    TaskStartDate = dto.TaskStartDate,
                    TaskDueDate = dto.TaskDueDate,
                    TaskEndDate = dto.TaskEndDate,
                    TaskCompletedTime = dto.TaskCompletedTime,
                    NextFollowUpDate = dto.NextFollowUpDate,
                    FacultyRemarks = dto.FacultyRemarks,
                    StudentRemarks = dto.StudentRemarks
                };

                _context.Task.Add(task);
                await _context.SaveChangesAsync();

                var dbTask = await _context.Task
                    .Include(t => t.ProjectAllocation)
                        .ThenInclude(pa => pa.ProjectMaster)
                    .Include(t => t.ProjectAllocation)
                        .ThenInclude(pa => pa.Student)
                    .Include(t => t.TaskStatus)
                    .Include(t => t.TaskPriority)
                    .FirstOrDefaultAsync(t => t.TaskID == task.TaskID);

                var response = new ReadTask
                {
                    TaskID = task.TaskID,
                    ProjectAllocationID = task.ProjectAllocationID,
                    ProjectTitle = dbTask?.ProjectAllocation?.ProjectMaster?.ProjectTitle,
                    StudentName = dbTask?.ProjectAllocation?.Student?.FullName,
                    TaskTitle = task.TaskTitle,
                    TaskDescription = task.TaskDescription,
                    TaskStatusID = task.TaskStatusID,
                    TaskStatusName = dbTask?.TaskStatus?.TaskStatusName,
                    TaskPriorityID = task.TaskPriorityID,
                    TaskPriorityName = dbTask?.TaskPriority?.TaskPriorityName,
                    AssignedScore = task.AssignedScore,
                    EarnedScore = task.EarnedScore,
                    ProgressPercentage = task.ProgressPercentage,
                    TaskAssignedDate = task.TaskAssignnedDate,
                    TaskStartDate = task.TaskStartDate,
                    TaskDueDate = task.TaskDueDate,
                    TaskEndDate = task.TaskEndDate,
                    TaskCompletedDate = task.TaskCompletedTime,
                    NextFollowUpDate = task.NextFollowUpDate,
                    FacultyRemarks = task.FacultyRemarks,
                    StudentRemarks = task.StudentRemarks
                };

                return CreatedAtAction(nameof(GetById), new { id = task.TaskID }, new ApiResponse<ReadTask>
                {
                    Success = true,
                    Message = "Task Added Successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadTask>
                {
                    Success = false,
                    Message = "Error occurred while adding task",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{id:int:min(1)}")]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Update(int id, UpdateTask dto)
        {
            var result = await _updateValidator.ValidateAsync(dto);

            if (!result.IsValid)
            {
                return BadRequest(new ApiResponse<ReadTask>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = result.Errors.Select(e => e.ErrorMessage).ToList()
                });
            }

            try
            {
                var existingTask = await _context.Task.FindAsync(id);

                if (existingTask == null)
                {
                    return NotFound(new ApiResponse<ReadTask>
                    {
                        Success = false,
                        Message = "Task Not Found",
                        Errors = new List<string> { $"No task found with Id {id}" }
                    });
                }

                existingTask.ProjectAllocationID = dto.ProjectAllocationID;
                existingTask.TaskTitle = dto.TaskTitle;
                existingTask.TaskDescription = dto.TaskDescription;
                existingTask.TaskStatusID = dto.TaskStatusID;
                existingTask.TaskPriorityID = dto.TaskPriorityID;
                existingTask.AssignedScore = dto.AssignedScore;
                existingTask.EarnedScore = dto.EarnedScore;
                existingTask.ProgressPercentage = dto.ProgressPercentage;
                existingTask.TaskAssignnedDate = dto.TaskAssignnedDate;
                existingTask.TaskStartDate = dto.TaskStartDate;
                existingTask.TaskDueDate = dto.TaskDueDate;
                existingTask.TaskEndDate = dto.TaskEndDate;
                existingTask.TaskCompletedTime = dto.TaskCompletedTime;
                existingTask.NextFollowUpDate = dto.NextFollowUpDate;
                existingTask.FacultyRemarks = dto.FacultyRemarks;
                existingTask.StudentRemarks = dto.StudentRemarks;

                await _context.SaveChangesAsync();

                var dbTask = await _context.Task
                    .Include(t => t.ProjectAllocation)
                        .ThenInclude(pa => pa.ProjectMaster)
                    .Include(t => t.ProjectAllocation)
                        .ThenInclude(pa => pa.Student)
                    .Include(t => t.TaskStatus)
                    .Include(t => t.TaskPriority)
                    .FirstOrDefaultAsync(t => t.TaskID == existingTask.TaskID);

                var response = new ReadTask
                {
                    TaskID = existingTask.TaskID,
                    ProjectAllocationID = existingTask.ProjectAllocationID,
                    ProjectTitle = dbTask?.ProjectAllocation?.ProjectMaster?.ProjectTitle,
                    StudentName = dbTask?.ProjectAllocation?.Student?.FullName,
                    TaskTitle = existingTask.TaskTitle,
                    TaskDescription = existingTask.TaskDescription,
                    TaskStatusID = existingTask.TaskStatusID,
                    TaskStatusName = dbTask?.TaskStatus?.TaskStatusName,
                    TaskPriorityID = existingTask.TaskPriorityID,
                    TaskPriorityName = dbTask?.TaskPriority?.TaskPriorityName,
                    AssignedScore = existingTask.AssignedScore,
                    EarnedScore = existingTask.EarnedScore,
                    ProgressPercentage = existingTask.ProgressPercentage,
                    TaskAssignedDate = existingTask.TaskAssignnedDate,
                    TaskStartDate = existingTask.TaskStartDate,
                    TaskDueDate = existingTask.TaskDueDate,
                    TaskEndDate = existingTask.TaskEndDate,
                    TaskCompletedDate = existingTask.TaskCompletedTime,
                    NextFollowUpDate = existingTask.NextFollowUpDate,
                    FacultyRemarks = existingTask.FacultyRemarks,
                    StudentRemarks = existingTask.StudentRemarks
                };

                return Ok(new ApiResponse<ReadTask>
                {
                    Success = true,
                    Message = "Task Updated Successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadTask>
                {
                    Success = false,
                    Message = "Error occurred while updating task",
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
                var task = await _context.Task.FindAsync(id);
                if (task == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Task Not Found",
                        Errors = new List<string> { $"No task found with Id {id}" }
                    });
                }

                _context.Task.Remove(task);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Task Deleted Successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while deleting task",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // Dropdown data for the task form (project allocations with student names)
        [HttpGet("FillDDLProjectAllocation")]
        [Authorize(Roles = "Admin,Faculty")]
        public List<SelectListItem> FillDDLProjectAllocation()
        {
            var userId = GetLoggedInUserId();
            var isAdmin = IsAdmin();

            var query = _context.ProjectAllocation
                .Include(pa => pa.ProjectMaster)
                .Include(pa => pa.Student)
                .AsQueryable();

            // Faculty only sees their own allocations in the dropdown
            if (!isAdmin)
            {
                query = query.Where(pa => pa.FacultyID == userId);
            }

            var allocations = query
                .Select(pa => new SelectListItem
                {
                    Value = pa.ProjectAllocationID.ToString(),
                    Text = pa.ProjectMaster.ProjectTitle + " - " + pa.Student.FullName
                }).ToList();
            if (allocations.Count == 0)
            {
                allocations.Add(new SelectListItem { Value = "", Text = "No Project Allocations Available" });
            }
            return allocations;
        }

        [HttpGet("FillDDLTaskStatus")]
        public List<SelectListItem> FillDDLTaskStatus()
        {
            var taskStatuses = _context.TaskStatus.Select(ts => new SelectListItem
            {
                Value = ts.TaskStatusID.ToString(),
                Text = ts.TaskStatusName
            }).ToList();
            if (taskStatuses.Count == 0)
            {
                taskStatuses.Add(new SelectListItem { Value = "", Text = "No Task Statuses Available" });
            }
            return taskStatuses;
        }

        [HttpGet("FillDDLTaskPriority")]
        public List<SelectListItem> FillDDLTaskPriority()
        {
            var taskPriorities = _context.TaskPriority.Select(tp => new SelectListItem
            {
                Value = tp.TaskPriorityId.ToString(),
                Text = tp.TaskPriorityName
            }).ToList();
            if (taskPriorities.Count == 0)
            {
                taskPriorities.Add(new SelectListItem { Value = "", Text = "No Task Priorities Available" });
            }
            return taskPriorities;
        }

        // Student progress update: progress, remarks and completion only
        [HttpPatch("{id:int:min(1)}/progress")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> UpdateProgress(int id, UpdateStudentTaskProgress dto)
        {
            if (dto.ProgressPercentage < 0 || dto.ProgressPercentage > 100)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = new List<string> { "Progress percentage must be between 0 and 100." }
                });
            }

            var userId = GetLoggedInUserId();
            var task = await _context.Task
                .Include(t => t.ProjectAllocation)
                .FirstOrDefaultAsync(t => t.TaskID == id && t.ProjectAllocation.StudentID == userId);
            if (task == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Task Not Found",
                    Errors = new List<string> { "The task does not exist or is not assigned to you." }
                });
            }

            task.ProgressPercentage = dto.ProgressPercentage;
            task.TaskStartDate = dto.TaskStartDate ?? task.TaskStartDate;
            task.TaskCompletedTime = dto.ProgressPercentage >= 100
                ? dto.TaskCompletedTime ?? DateTime.UtcNow
                : null;
            task.StudentRemarks = dto.StudentRemarks;

            // Keep the status aligned with the reported progress
            var statusName = dto.ProgressPercentage >= 100 ? "Completed"
                : dto.ProgressPercentage > 0 ? "In Progress" : "Pending";
            var matchingStatus = await _context.TaskStatus
                .FirstOrDefaultAsync(s => s.TaskStatusName == statusName);
            if (matchingStatus != null)
            {
                task.TaskStatusID = matchingStatus.TaskStatusID;
            }

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Task progress updated successfully",
                Data = null
            });
        }
    }
}