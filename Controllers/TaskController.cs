using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPMS_API.Common;
using SPMS_API.Data;
using SPMS_API.DTOs;

namespace SPMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var tasks = await _context.Task
                    .Include(t => t.ProjectAllocation)
                        .ThenInclude(pa => pa.ProjectMaster)
                    .Include(t => t.ProjectAllocation)
                        .ThenInclude(pa => pa.Student)
                    .Include(t => t.TaskStatus)
                    .Include(t => t.TaskPriority)
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
                var task = await _context.Task
                    .Include(t => t.ProjectAllocation)
                        .ThenInclude(pa => pa.ProjectMaster)
                    .Include(t => t.ProjectAllocation)
                        .ThenInclude(pa => pa.Student)
                    .Include(t => t.TaskStatus)
                    .Include(t => t.TaskPriority)
                    .Where(t => t.TaskID == id)
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
        public async Task<IActionResult> Add(CreateTask dto)
        {
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

                var result = new ReadTask
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
                    Data = result
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
        public async Task<IActionResult> Update(int id, UpdateTask dto)
        {
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

                var result = new ReadTask
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
                    Data = result
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
    }
}