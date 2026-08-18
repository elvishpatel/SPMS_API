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
            try
            {
                var projects = await _context.ProjectMaster
                    .Select(p => new ReadProjectMaster
                    {
                        ProjectId = p.ProjectId,
                        ProjectTitle = p.ProjectTitle,
                        Description = p.Description
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<List<ReadProjectMaster>>
                {
                    Success = true,
                    Message = "Projects Retrieved Successfully",
                    Data = projects
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<ReadProjectMaster>>
                {
                    Success = false,
                    Message = "Error occurred while retrieving projects",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var project = await _context.ProjectMaster
                    .Where(p => p.ProjectId == id)
                    .Select(p => new ReadProjectMaster
                    {
                        ProjectId = p.ProjectId,
                        ProjectTitle = p.ProjectTitle,
                        Description = p.Description
                    })
                    .FirstOrDefaultAsync();

                if (project == null)
                {
                    return NotFound(new ApiResponse<ReadProjectMaster>
                    {
                        Success = false,
                        Message = "Project Not Found",
                        Errors = new List<string> { $"No project found with Id {id}" }
                    });
                }

                return Ok(new ApiResponse<ReadProjectMaster>
                {
                    Success = true,
                    Message = "Project Retrieved Successfully",
                    Data = project
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadProjectMaster>
                {
                    Success = false,
                    Message = "Error occurred while retrieving project",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(CreateProjectMaster dto)
        {
            try
            {
                var project = new ProjectMaster
                {
                    ProjectTitle = dto.ProjectTitle,
                    Description = dto.Description
                };

                _context.ProjectMaster.Add(project);
                await _context.SaveChangesAsync();

                var response = new ReadProjectMaster
                {
                    ProjectId = project.ProjectId,
                    ProjectTitle = project.ProjectTitle,
                    Description = project.Description
                };

                return CreatedAtAction(nameof(GetById), new { id = project.ProjectId }, new ApiResponse<ReadProjectMaster>
                {
                    Success = true,
                    Message = "Project Added Successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadProjectMaster>
                {
                    Success = false,
                    Message = "Error occurred while adding project",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<IActionResult> Update(int id, UpdateProjectMaster dto)
        {
            try
            {
                var project = await _context.ProjectMaster.FindAsync(id);

                if (project == null)
                {
                    return NotFound(new ApiResponse<ReadProjectMaster>
                    {
                        Success = false,
                        Message = "Project Not Found",
                        Errors = new List<string> { $"No project found with Id {id}" }
                    });
                }

                project.ProjectTitle = dto.ProjectTitle;
                project.Description = dto.Description;

                await _context.SaveChangesAsync();

                var response = new ReadProjectMaster
                {
                    ProjectId = project.ProjectId,
                    ProjectTitle = project.ProjectTitle,
                    Description = project.Description
                };

                return Ok(new ApiResponse<ReadProjectMaster>
                {
                    Success = true,
                    Message = "Project Updated Successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadProjectMaster>
                {
                    Success = false,
                    Message = "Error occurred while updating project",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var project = await _context.ProjectMaster.FindAsync(id);

                if (project == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Project Not Found",
                        Errors = new List<string> { $"No project found with Id {id}" }
                    });
                }

                _context.ProjectMaster.Remove(project);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Project Deleted Successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while deleting project",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}