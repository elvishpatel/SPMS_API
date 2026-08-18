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
    public class TaskStatusController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskStatusController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var taskStatuses = await _context.TaskStatus
                    .Select(ts => new ReadTaskStatus
                    {
                        TaskStatusID = ts.TaskStatusID,
                        TaskStatusName = ts.TaskStatusName,
                        TaskStatusCssClass = ts.TaskStatusCssClass
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<List<ReadTaskStatus>>
                {
                    Success = true,
                    Message = "Task Statuses Retrieved Successfully",
                    Data = taskStatuses
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<ReadTaskStatus>>
                {
                    Success = false,
                    Message = "Error occurred while retrieving task statuses",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var taskStatus = await _context.TaskStatus
                    .Where(ts => ts.TaskStatusID == id)
                    .Select(ts => new ReadTaskStatus
                    {
                        TaskStatusID = ts.TaskStatusID,
                        TaskStatusName = ts.TaskStatusName,
                        TaskStatusCssClass = ts.TaskStatusCssClass
                    })
                    .FirstOrDefaultAsync();

                if (taskStatus == null)
                {
                    return NotFound(new ApiResponse<ReadTaskStatus>
                    {
                        Success = false,
                        Message = "Task Status Not Found",
                        Errors = new List<string> { $"No task status found with Id {id}" }
                    });
                }

                return Ok(new ApiResponse<ReadTaskStatus>
                {
                    Success = true,
                    Message = "Task Status Retrieved Successfully",
                    Data = taskStatus
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadTaskStatus>
                {
                    Success = false,
                    Message = "Error occurred while retrieving task status",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(CreateTaskStatus dto)
        {
            try
            {
                var taskStatus = new Models.TaskStatus
                {
                    TaskStatusName = dto.TaskStatusName,
                    TaskStatusCssClass = dto.TaskStatusCssClass
                };

                _context.TaskStatus.Add(taskStatus);
                await _context.SaveChangesAsync();

                var response = new ReadTaskStatus
                {
                    TaskStatusID = taskStatus.TaskStatusID,
                    TaskStatusName = taskStatus.TaskStatusName,
                    TaskStatusCssClass = taskStatus.TaskStatusCssClass
                };

                return CreatedAtAction(nameof(GetById), new { id = taskStatus.TaskStatusID }, new ApiResponse<ReadTaskStatus>
                {
                    Success = true,
                    Message = "Task Status Added Successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadTaskStatus>
                {
                    Success = false,
                    Message = "Error occurred while adding task status",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<IActionResult> Update(int id, UpdateTaskStatus dto)
        {
            try
            {
                var taskStatus = await _context.TaskStatus.FindAsync(id);

                if (taskStatus == null)
                {
                    return NotFound(new ApiResponse<ReadTaskStatus>
                    {
                        Success = false,
                        Message = "Task Status Not Found",
                        Errors = new List<string> { $"No task status found with Id {id}" }
                    });
                }

                taskStatus.TaskStatusName = dto.TaskStatusName;
                taskStatus.TaskStatusCssClass = dto.TaskStatusCssClass;

                await _context.SaveChangesAsync();

                var response = new ReadTaskStatus
                {
                    TaskStatusID = taskStatus.TaskStatusID,
                    TaskStatusName = taskStatus.TaskStatusName,
                    TaskStatusCssClass = taskStatus.TaskStatusCssClass
                };

                return Ok(new ApiResponse<ReadTaskStatus>
                {
                    Success = true,
                    Message = "Task Status Updated Successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadTaskStatus>
                {
                    Success = false,
                    Message = "Error occurred while updating task status",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var taskStatus = await _context.TaskStatus.FindAsync(id);

                if (taskStatus == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Task Status Not Found",
                        Errors = new List<string> { $"No task status found with Id {id}" }
                    });
                }

                _context.TaskStatus.Remove(taskStatus);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Task Status Deleted Successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while deleting task status",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}