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
    public class TaskPriorityController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskPriorityController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var taskPriorities = await _context.TaskPriority
                    .Select(tp => new ReadTaskPriority
                    {
                        TaskPriorityId = tp.TaskPriorityId,
                        TaskPriorityName = tp.TaskPriorityName,
                        TaskPriorityCssClass = tp.TaskPriorityCssClass
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<List<ReadTaskPriority>>
                {
                    Success = true,
                    Message = "Task Priorities Retrieved Successfully",
                    Data = taskPriorities
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<ReadTaskPriority>>
                {
                    Success = false,
                    Message = "Error occurred while retrieving task priorities",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var taskPriority = await _context.TaskPriority
                    .Where(tp => tp.TaskPriorityId == id)
                    .Select(tp => new ReadTaskPriority
                    {
                        TaskPriorityId = tp.TaskPriorityId,
                        TaskPriorityName = tp.TaskPriorityName,
                        TaskPriorityCssClass = tp.TaskPriorityCssClass
                    })
                    .FirstOrDefaultAsync();

                if (taskPriority == null)
                {
                    return NotFound(new ApiResponse<ReadTaskPriority>
                    {
                        Success = false,
                        Message = "Task Priority Not Found",
                        Errors = new List<string> { $"No task priority found with Id {id}" }
                    });
                }

                return Ok(new ApiResponse<ReadTaskPriority>
                {
                    Success = true,
                    Message = "Task Priority Retrieved Successfully",
                    Data = taskPriority
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadTaskPriority>
                {
                    Success = false,
                    Message = "Error occurred while retrieving task priority",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(CreateTaskPriority dto)
        {
            try
            {
                var taskPriority = new TaskPriority
                {
                    TaskPriorityName = dto.TaskPriorityName,
                    TaskPriorityCssClass = dto.TaskPriorityCssClass
                };

                _context.TaskPriority.Add(taskPriority);
                await _context.SaveChangesAsync();

                var response = new ReadTaskPriority
                {
                    TaskPriorityId = taskPriority.TaskPriorityId,
                    TaskPriorityName = taskPriority.TaskPriorityName,
                    TaskPriorityCssClass = taskPriority.TaskPriorityCssClass
                };

                return CreatedAtAction(nameof(GetById), new { id = taskPriority.TaskPriorityId }, new ApiResponse<ReadTaskPriority>
                {
                    Success = true,
                    Message = "Task Priority Added Successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadTaskPriority>
                {
                    Success = false,
                    Message = "Error occurred while adding task priority",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<IActionResult> Update(int id, UpdateTaskPriority dto)
        {
            try
            {
                var taskPriority = await _context.TaskPriority.FindAsync(id);

                if (taskPriority == null)
                {
                    return NotFound(new ApiResponse<ReadTaskPriority>
                    {
                        Success = false,
                        Message = "Task Priority Not Found",
                        Errors = new List<string> { $"No task priority found with Id {id}" }
                    });
                }

                taskPriority.TaskPriorityName = dto.TaskPriorityName;
                taskPriority.TaskPriorityCssClass = dto.TaskPriorityCssClass;

                await _context.SaveChangesAsync();

                var response = new ReadTaskPriority
                {
                    TaskPriorityId = taskPriority.TaskPriorityId,
                    TaskPriorityName = taskPriority.TaskPriorityName,
                    TaskPriorityCssClass = taskPriority.TaskPriorityCssClass
                };

                return Ok(new ApiResponse<ReadTaskPriority>
                {
                    Success = true,
                    Message = "Task Priority Updated Successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadTaskPriority>
                {
                    Success = false,
                    Message = "Error occurred while updating task priority",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var taskPriority = await _context.TaskPriority.FindAsync(id);

                if (taskPriority == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Task Priority Not Found",
                        Errors = new List<string> { $"No task priority found with Id {id}" }
                    });
                }

                _context.TaskPriority.Remove(taskPriority);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Task Priority Deleted Successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while deleting task priority",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}