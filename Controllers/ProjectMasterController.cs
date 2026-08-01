using Microsoft.AspNetCore.Mvc;
using SPMS_API.Data;
using SPMS_API.Models;
using Microsoft.EntityFrameworkCore;

namespace SPMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectMasterController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectMasterController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var projects = await _context.ProjectMaster.ToListAsync();
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var project = await _context.ProjectMaster.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ProjectMaster project)
        {
            project.ProjectId = 0;
            _context.ProjectMaster.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = project.ProjectId }, project);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProjectMaster project)
        {
            if (project.ProjectId != 0 && project.ProjectId != id)
            {
                return BadRequest("ID in route parameter does not match ID in request body.");
            }

            var existingProject = await _context.ProjectMaster.FindAsync(id);

            if (existingProject == null)
            {
                return NotFound();
            }

            existingProject.ProjectTitle = project.ProjectTitle;
            existingProject.Description = project.Description;

            await _context.SaveChangesAsync();

            return Ok(existingProject);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _context.ProjectMaster.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            _context.ProjectMaster.Remove(project);
            await _context.SaveChangesAsync();

            return Ok("Project deleted successfully.");
        }
    }
}