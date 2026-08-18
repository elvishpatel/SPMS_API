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
    public class ProjectAllocationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectAllocationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var projectAllocations = await _context.ProjectAllocation
                    .Include(p => p.ProjectMaster)
                    .Include(p => p.Student)
                    .Include(p => p.Faculty)
                    .Select(p => new ReadProjectAllocation
                    {
                        ProjectAllocationID = p.ProjectAllocationID,
                        ProjectID = p.ProjectID,
                        ProjectTitle = p.ProjectMaster != null ? p.ProjectMaster.ProjectTitle : null,
                        StudentID = p.StudentID,
                        StudentName = p.Student != null ? p.Student.FullName : null,
                        FacultyID = p.FacultyID,
                        FacultyName = p.Faculty != null ? p.Faculty.FullName : null,
                        AssignedDate = p.AssignedDate,
                        ProjectStartDate = p.ProjectStartDate,
                        ProjectEndDate = p.ProjectEndDate,
                        TotalTasksGiven = p.TotalTasksGiven,
                        TotalCompletedTasks = p.TotalCompletedTasks,
                        ProgressPercentage = p.ProgressPercentage,
                        OverAllGrade = p.OverAllGrade
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<List<ReadProjectAllocation>>
                {
                    Success = true,
                    Message = "Project Allocations Retrieved Successfully",
                    Data = projectAllocations
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<ReadProjectAllocation>>
                {
                    Success = false,
                    Message = "Error occurred while retrieving project allocations",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var projectAllocation = await _context.ProjectAllocation
                    .Include(p => p.ProjectMaster)
                    .Include(p => p.Student)
                    .Include(p => p.Faculty)
                    .Where(p => p.ProjectAllocationID == id)
                    .Select(p => new ReadProjectAllocation
                    {
                        ProjectAllocationID = p.ProjectAllocationID,
                        ProjectID = p.ProjectID,
                        ProjectTitle = p.ProjectMaster != null ? p.ProjectMaster.ProjectTitle : null,
                        StudentID = p.StudentID,
                        StudentName = p.Student != null ? p.Student.FullName : null,
                        FacultyID = p.FacultyID,
                        FacultyName = p.Faculty != null ? p.Faculty.FullName : null,
                        AssignedDate = p.AssignedDate,
                        ProjectStartDate = p.ProjectStartDate,
                        ProjectEndDate = p.ProjectEndDate,
                        TotalTasksGiven = p.TotalTasksGiven,
                        TotalCompletedTasks = p.TotalCompletedTasks,
                        ProgressPercentage = p.ProgressPercentage,
                        OverAllGrade = p.OverAllGrade
                    })
                    .FirstOrDefaultAsync();

                if (projectAllocation == null)
                {
                    return NotFound(new ApiResponse<ReadProjectAllocation>
                    {
                        Success = false,
                        Message = "Project Allocation Not Found",
                        Errors = new List<string> { $"No project allocation found with Id {id}" }
                    });
                }

                return Ok(new ApiResponse<ReadProjectAllocation>
                {
                    Success = true,
                    Message = "Project Allocation Retrieved Successfully",
                    Data = projectAllocation
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadProjectAllocation>
                {
                    Success = false,
                    Message = "Error occurred while retrieving project allocation",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(CreateProjectAllocation dto)
        {
            try
            {
                var projectAllocation = new ProjectAllocation
                {
                    ProjectID = dto.ProjectID,
                    StudentID = dto.StudentID,
                    FacultyID = dto.FacultyID,
                    AssignedDate = dto.AssignedDate,
                    ProjectStartDate = dto.ProjectStartDate,
                    ProjectEndDate = dto.ProjectEndDate,
                    TotalTasksGiven = dto.TotalTasksGiven,
                    TotalCompletedTasks = dto.TotalCompletedTasks,
                    ProgressPercentage = dto.ProgressPercentage,
                    OverAllGrade = dto.OverAllGrade
                };

                _context.ProjectAllocation.Add(projectAllocation);
                await _context.SaveChangesAsync();

                var dbAlloc = await _context.ProjectAllocation
                    .Include(p => p.ProjectMaster)
                    .Include(p => p.Student)
                    .Include(p => p.Faculty)
                    .FirstOrDefaultAsync(p => p.ProjectAllocationID == projectAllocation.ProjectAllocationID);

                var response = new ReadProjectAllocation
                {
                    ProjectAllocationID = projectAllocation.ProjectAllocationID,
                    ProjectID = projectAllocation.ProjectID,
                    ProjectTitle = dbAlloc?.ProjectMaster?.ProjectTitle,
                    StudentID = projectAllocation.StudentID,
                    StudentName = dbAlloc?.Student?.FullName,
                    FacultyID = projectAllocation.FacultyID,
                    FacultyName = dbAlloc?.Faculty?.FullName,
                    AssignedDate = projectAllocation.AssignedDate,
                    ProjectStartDate = projectAllocation.ProjectStartDate,
                    ProjectEndDate = projectAllocation.ProjectEndDate,
                    TotalTasksGiven = projectAllocation.TotalTasksGiven,
                    TotalCompletedTasks = projectAllocation.TotalCompletedTasks,
                    ProgressPercentage = projectAllocation.ProgressPercentage,
                    OverAllGrade = projectAllocation.OverAllGrade
                };

                return CreatedAtAction(nameof(GetById), new { id = projectAllocation.ProjectAllocationID }, new ApiResponse<ReadProjectAllocation>
                {
                    Success = true,
                    Message = "Project Allocation Added Successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadProjectAllocation>
                {
                    Success = false,
                    Message = "Error occurred while adding project allocation",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<IActionResult> Update(int id, UpdateProjectAllocation dto)
        {
            try
            {
                var existingProjectAllocation = await _context.ProjectAllocation.FindAsync(id);

                if (existingProjectAllocation == null)
                {
                    return NotFound(new ApiResponse<ReadProjectAllocation>
                    {
                        Success = false,
                        Message = "Project Allocation Not Found",
                        Errors = new List<string> { $"No project allocation found with Id {id}" }
                    });
                }

                existingProjectAllocation.ProjectID = dto.ProjectID;
                existingProjectAllocation.StudentID = dto.StudentID;
                existingProjectAllocation.FacultyID = dto.FacultyID;
                existingProjectAllocation.AssignedDate = dto.AssignedDate;
                existingProjectAllocation.ProjectStartDate = dto.ProjectStartDate;
                existingProjectAllocation.ProjectEndDate = dto.ProjectEndDate;
                existingProjectAllocation.TotalTasksGiven = dto.TotalTasksGiven;
                existingProjectAllocation.TotalCompletedTasks = dto.TotalCompletedTasks;
                existingProjectAllocation.ProgressPercentage = dto.ProgressPercentage;
                existingProjectAllocation.OverAllGrade = dto.OverAllGrade;

                await _context.SaveChangesAsync();

                var dbAlloc = await _context.ProjectAllocation
                    .Include(p => p.ProjectMaster)
                    .Include(p => p.Student)
                    .Include(p => p.Faculty)
                    .FirstOrDefaultAsync(p => p.ProjectAllocationID == existingProjectAllocation.ProjectAllocationID);

                var response = new ReadProjectAllocation
                {
                    ProjectAllocationID = existingProjectAllocation.ProjectAllocationID,
                    ProjectID = existingProjectAllocation.ProjectID,
                    ProjectTitle = dbAlloc?.ProjectMaster?.ProjectTitle,
                    StudentID = existingProjectAllocation.StudentID,
                    StudentName = dbAlloc?.Student?.FullName,
                    FacultyID = existingProjectAllocation.FacultyID,
                    FacultyName = dbAlloc?.Faculty?.FullName,
                    AssignedDate = existingProjectAllocation.AssignedDate,
                    ProjectStartDate = existingProjectAllocation.ProjectStartDate,
                    ProjectEndDate = existingProjectAllocation.ProjectEndDate,
                    TotalTasksGiven = existingProjectAllocation.TotalTasksGiven,
                    TotalCompletedTasks = existingProjectAllocation.TotalCompletedTasks,
                    ProgressPercentage = existingProjectAllocation.ProgressPercentage,
                    OverAllGrade = existingProjectAllocation.OverAllGrade
                };

                return Ok(new ApiResponse<ReadProjectAllocation>
                {
                    Success = true,
                    Message = "Project Allocation Updated Successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ReadProjectAllocation>
                {
                    Success = false,
                    Message = "Error occurred while updating project allocation",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var projectAllocation = await _context.ProjectAllocation.FindAsync(id);

                if (projectAllocation == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Project Allocation Not Found",
                        Errors = new List<string> { $"No project allocation found with Id {id}" }
                    });
                }

                _context.ProjectAllocation.Remove(projectAllocation);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Project Allocation Deleted Successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while deleting project allocation",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}