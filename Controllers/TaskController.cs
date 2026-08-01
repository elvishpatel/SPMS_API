using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SPMS_API.Data;
using SPMS_API.Models;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<SPMS_API.Models.Task>> GetALl()
        {
            var Tasks = await _context.Task.ToListAsync();
            return Ok(Tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SPMS_API.Models.Task>> GetById(int id)
        {
            var task = await _context.Task.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }
        [HttpPost]
        public async Task<ActionResult<SPMS_API.Models.Task>> Add(SPMS_API.Models.Task task)
        {
            task.TaskID = 0;
            _context.Task.Add(task);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = task.TaskID }, task);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<SPMS_API.Models.Task>> Update(int id, SPMS_API.Models.Task task)
        {

            if (task.TaskID != 0 && task.TaskID != id)
            {
                return BadRequest("ID in route parameter does not match ID in request body.");
            }

            var existingTask = await _context.Task.FindAsync(id);

            if (existingTask == null)
            {
                return NotFound();
            }

            existingTask.ProjectAllocationID = task.ProjectAllocationID;
            existingTask.TaskTitle = task.TaskTitle;
            existingTask.TaskDescription = task.TaskDescription;
            existingTask.TaskStatusID = task.TaskStatusID;
            existingTask.TaskPriorityID = task.TaskPriorityID;
            existingTask.AssignedScore = task.AssignedScore;
            existingTask.EarnedScore = task.EarnedScore;
            existingTask.ProgressPercentage = task.ProgressPercentage;
            existingTask.TaskAssignnedDate = task.TaskAssignnedDate;
            existingTask.TaskStartDate = task.TaskStartDate;
            existingTask.TaskEndDate = task.TaskEndDate;
            existingTask.TaskCompletedTime = task.TaskCompletedTime;
            existingTask.NextFollowUpDate = task.NextFollowUpDate;
            existingTask.FacultyRemarks = task.FacultyRemarks;
            existingTask.StudentRemarks = task.StudentRemarks;

            await _context.SaveChangesAsync();

            return Ok(existingTask);


        }
        [HttpDelete("{id}")]

        public async Task<ActionResult<SPMS_API.Models.Task>> Delete(int id)
        {
            var task = await _context.Task.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            _context.Task.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
