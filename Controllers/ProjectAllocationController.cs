using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPMS_API.Data;
using SPMS_API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectAllocationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectAllocationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ProjectAllocation>> GetALl()
        {
            var projectAllocations = await _context.ProjectAllocation.ToListAsync();
            return Ok(projectAllocations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectAllocation>> GetById(int id)
        {
            var projectAllocation = await _context.ProjectAllocation.FindAsync(id);
            if (projectAllocation == null)
            {
                return NotFound();
            }
            return Ok(projectAllocation);
        }
        [HttpPost]
        public async Task<ActionResult<ProjectAllocation>> Add(ProjectAllocation projectAllocation)
        {
            projectAllocation.ProjectAllocationID = 0;
            _context.ProjectAllocation.Add(projectAllocation);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = projectAllocation.ProjectAllocationID }, projectAllocation);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<ProjectAllocation>> Update(int id, ProjectAllocation projectAllocation)
        {

            if (projectAllocation.ProjectAllocationID != 0 && projectAllocation.ProjectAllocationID != id)
            {
                return BadRequest("ID in route parameter does not match ID in request body.");
            }

            var existingProjectAllocation = await _context.ProjectAllocation.FindAsync(id);

            if (existingProjectAllocation == null)
            {
                return NotFound();
            }

            existingProjectAllocation.ProjectID = projectAllocation.ProjectID;
            existingProjectAllocation.StudentID = projectAllocation.StudentID;
            existingProjectAllocation.FacultyID = projectAllocation.FacultyID;
            existingProjectAllocation.AssignedDate = projectAllocation.AssignedDate;
            existingProjectAllocation.ProjectStartDate = projectAllocation.ProjectStartDate;
            existingProjectAllocation.ProjectEndDate = projectAllocation.ProjectEndDate;
            existingProjectAllocation.TotalTasksGiven = projectAllocation.TotalTasksGiven;
            existingProjectAllocation.TotalCompletedTasks = projectAllocation.TotalCompletedTasks;
            existingProjectAllocation.ProgressPercentage = projectAllocation.ProgressPercentage;
            existingProjectAllocation.OverAllGrade = projectAllocation.OverAllGrade;


            await _context.SaveChangesAsync();

            return Ok(existingProjectAllocation);


        }
        [HttpDelete("{id}")]

        public async Task<ActionResult<SPMS_API.Models.ProjectAllocation>> Delete(int id)
        {
            var projectAllocation = await _context.ProjectAllocation.FindAsync(id);
            if (projectAllocation == null)
            {
                return NotFound();
            }
            _context.ProjectAllocation.Remove(projectAllocation);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}