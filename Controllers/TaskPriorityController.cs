using Microsoft.AspNetCore.Mvc;
using SPMS_API.Data;
using SPMS_API.Models;
using Microsoft.EntityFrameworkCore;

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
            var taskPriorities =  await _context.TaskPriority.ToListAsync();
            return Ok(taskPriorities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var taskPriority = await _context.TaskPriority.FindAsync(id);

            if (taskPriority == null)
            {
                return NotFound();
            }

            return Ok(taskPriority);
        }

        [HttpPost]
        public async Task<IActionResult> Add(TaskPriority taskPriority)
        {
            taskPriority.TaskPriorityId = 0;
            _context.TaskPriority.Add(taskPriority);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = taskPriority.TaskPriorityId }, taskPriority);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TaskPriority taskPriority)
        {
            if (taskPriority.TaskPriorityId != 0 && taskPriority.TaskPriorityId != id)
            {
                return BadRequest("ID in route parameter does not match ID in request body.");
            }

            var existingTaskPriority = await _context.TaskPriority.FindAsync(id);

            if (existingTaskPriority == null)
            {
                return NotFound();
            }

            existingTaskPriority.TaskPriorityName = taskPriority.TaskPriorityName;
            existingTaskPriority.TaskPriorityCssClass = taskPriority.TaskPriorityCssClass;

            await _context.SaveChangesAsync();

            return Ok(existingTaskPriority);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var taskPriority = await _context.TaskPriority.FindAsync(id);

            if (taskPriority == null)
            {
                return NotFound();
            }

            _context.TaskPriority.Remove(taskPriority);
            await _context.SaveChangesAsync();

            return Ok("Task Priority deleted successfully.");
        }
    }
}