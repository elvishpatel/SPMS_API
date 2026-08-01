using Microsoft.AspNetCore.Mvc;
using SPMS_API.Data;
using SPMS_API.Models;
using Microsoft.EntityFrameworkCore;

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
            var taskStatuses = await _context.TaskStatus.ToListAsync();
            return Ok(taskStatuses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var taskStatus = await _context.TaskStatus.FindAsync(id);

            if (taskStatus == null)
            {
                return NotFound();
            }

            return Ok(taskStatus);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Models.TaskStatus taskStatus)
        {
            taskStatus.TaskStatusID = 0;
            _context.TaskStatus.Add(taskStatus);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = taskStatus.TaskStatusID }, taskStatus);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Models.TaskStatus taskStatus)
        {
            if (taskStatus.TaskStatusID != 0 && taskStatus.TaskStatusID != id)
            {
                return BadRequest("ID in route parameter does not match ID in request body.");
            }

            var existingTaskStatus = await _context.TaskStatus.FindAsync(id);

            if (existingTaskStatus == null)
            {
                return NotFound();
            }

            existingTaskStatus.TaskStatusName = taskStatus.TaskStatusName;
            existingTaskStatus.TaskStatusCssClass = taskStatus.TaskStatusCssClass;

            await _context.SaveChangesAsync();

            return Ok(existingTaskStatus);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var taskStatus = await _context.TaskStatus.FindAsync(id);

            if (taskStatus == null)
            {
                return NotFound();
            }

            _context.TaskStatus.Remove(taskStatus);
            await _context.SaveChangesAsync();

            return Ok("Task Status deleted successfully.");
        }
    }
}