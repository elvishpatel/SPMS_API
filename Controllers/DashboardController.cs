using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPMS_API.Common;
using SPMS_API.Data;
using System.Diagnostics.CodeAnalysis;

namespace SPMS_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // 1) Display the total number of students registered in the system.
        [HttpGet]
        public async Task<IActionResult> GetTotalStudents()
        {
            try
            {
                var totalStudents = await _context.User
                    .CountAsync(x => x.UserType.UserTypeName == "Student");

                return Ok(new ApiResponse<int>
                {
                    Success = true,
                    Message = "Total Students Retrieved Successfully",
                    Data = totalStudents
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<int>
                {
                    Success = false,
                    Message = "Error occurred while retrieving total students",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 2) Display the total number of faculty members guiding projects.
        [HttpGet]
        public async Task<IActionResult> GetTotalFaculty()
        {
            try
            {
                var totalFaculty = await _context.User
                    .CountAsync(x => x.UserType.UserTypeName == "Faculty");

                return Ok(new ApiResponse<int>
                {
                    Success = true,
                    Message = "Total Faculty Retrieved Successfully",
                    Data = totalFaculty
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<int>
                {
                    Success = false,
                    Message = "Error occurred while retrieving total faculty",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 3) Display the total number of projects available in the system.
        [HttpGet]
        public async Task<IActionResult> GetTotalProjects()
        {
            try
            {
                var totalProjects = await _context.ProjectMaster.CountAsync();

                return Ok(new ApiResponse<int>
                {
                    Success = true,
                    Message = "Total Projects Retrieved Successfully",
                    Data = totalProjects
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<int>
                {
                    Success = false,
                    Message = "Error occurred while retrieving total projects",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 4) Show how many tasks belong to each status category.
        [HttpGet]
        public async Task<IActionResult> GetTaskStatusSummary()
        {
            try
            {
                var taskStatusSummary = await _context.Task
                    .GroupBy(t => t.TaskStatus.TaskStatusName)
                    .Select(g => new
                    {
                        TaskStatus = g.Key,
                        TotalTasks = g.Count()
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Task Status Summary Retrieved Successfully",
                    Data = taskStatusSummary
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving task status summary",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 5) Show priority wise task count
        [HttpGet]
        public async Task<IActionResult> GetPrioritySummary()
        {
            try
            {
                var prioritySummary = await _context.Task
                    .GroupBy(t => t.TaskPriority.TaskPriorityName)
                    .Select(g => new
                    {
                        Priority = g.Key,
                        TotalTasks = g.Count()
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Priority Summary Retrieved Successfully",
                    Data = prioritySummary
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving priority summary",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 6) Show how many projects are assigned to each faculty member.
        [HttpGet]
        public async Task<IActionResult> GetFacultyWorkload()
        {
            try
            {
                var facultyWorkload = await _context.ProjectAllocation
                    .GroupBy(p => p.Faculty.FullName)
                    .Select(g => new
                    {
                        FacultyName = g.Key,
                        TotalProjects = g.Count()
                    })
                    .OrderByDescending(x => x.TotalProjects)
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Faculty Workload Retrieved Successfully",
                    Data = facultyWorkload
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving faculty workload",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 7) Show how many tasks have been assigned to each student.
        [HttpGet]
        public async Task<IActionResult> GetStudentTasks()
        {
            try
            {
                var studentTasks = await _context.Task
                    .GroupBy(t => t.ProjectAllocation.Student.FullName)
                    .Select(g => new
                    {
                        StudentName = g.Key,
                        TotalTasks = g.Count()
                    })
                    .OrderByDescending(x => x.TotalTasks)
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Student Tasks Retrieved Successfully",
                    Data = studentTasks
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving student tasks",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 8) Display the top 10 students having the highest average earned score.
        [HttpGet]
        public async Task<IActionResult> GetTopStudents()
        {
            try
            {
                var topStudents = await _context.Task
                    .Where(t => t.EarnedScore != null)
                    .GroupBy(t => t.ProjectAllocation.Student.FullName)
                    .Select(g => new
                    {
                        StudentName = g.Key,
                        AverageScore = g.Average(t => t.EarnedScore)
                    })
                    .OrderByDescending(x => x.AverageScore)
                    .Take(10)
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Top Students Retrieved Successfully",
                    Data = topStudents
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving top students",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 9) Display the bottom 10 students based on average earned score.
        [HttpGet]
        public async Task<IActionResult> GetBottomStudents()
        {
            try
            {
                var bottomStudents = await _context.Task
                    .Where(t => t.EarnedScore != null)
                    .GroupBy(t => t.ProjectAllocation.Student.FullName)
                    .Select(g => new
                    {
                        StudentName = g.Key,
                        AverageScore = g.Average(t => t.EarnedScore)
                    })
                    .OrderBy(x => x.AverageScore)
                    .Take(10)
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Bottom Students Retrieved Successfully",
                    Data = bottomStudents
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving bottom students",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 10) Display all tasks whose due date has passed but are not completed.
        [HttpGet]
        public async Task<IActionResult> GetOverdueTasks()
        {
            try
            {
                var overdueTasks = await _context.Task
                    .Where(t =>
                        t.TaskDueDate < DateTime.Now &&
                        t.TaskStatus.TaskStatusName != "Completed")
                    .Select(t => new
                    {
                        TaskID = t.TaskID,
                        TaskTitle = t.TaskTitle,
                        Student = t.ProjectAllocation.Student.FullName,
                        Faculty = t.ProjectAllocation.Faculty.FullName,
                        TaskDueDate = t.TaskDueDate,
                        DaysOverdue = t.TaskDueDate.HasValue ? EF.Functions.DateDiffDay(t.TaskDueDate.Value, DateTime.Now) : 0
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Overdue Tasks Retrieved Successfully",
                    Data = overdueTasks
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving overdue tasks",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 11) Display tasks having follow-up dates within the next 7 days.
        [HttpGet]
        public async Task<IActionResult> GetUpcomingFollowUps()
        {
            try
            {
                var upcomingFollowUps = await _context.Task
                    .Where(t =>
                        t.NextFollowUpDate >= DateTime.Today &&
                        t.NextFollowUpDate <= DateTime.Today.AddDays(7))
                    .Select(t => new
                    {
                        TaskTitle = t.TaskTitle,
                        Student = t.ProjectAllocation.Student.FullName,
                        Faculty = t.ProjectAllocation.Faculty.FullName,
                        NextFollowUpDate = t.NextFollowUpDate
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Upcoming Follow-Ups Retrieved Successfully",
                    Data = upcomingFollowUps
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving upcoming follow-ups",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 12) Show how many students have obtained each grade.
        [HttpGet]
        public async Task<IActionResult> GetGradeDistribution()
        {
            try
            {
                var gradeDistribution = await _context.ProjectAllocation
                    .GroupBy(p => p.OverAllGrade)
                    .Select(g => new
                    {
                        Grade = g.Key,
                        Students = g.Count()
                    })
                    .OrderBy(x => x.Grade)
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Grade Distribution Retrieved Successfully",
                    Data = gradeDistribution
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving grade distribution",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 13) Show month-wise completed task count.
        [HttpGet]
        public async Task<IActionResult> GetMonthlyCompletion()
        {
            try
            {
                var monthlyCompletion = await _context.Task
                    .Where(t => t.TaskCompletedTime != null)
                    .GroupBy(t => new
                    {
                        Year = t.TaskCompletedTime.Value.Year,
                        Month = t.TaskCompletedTime.Value.Month
                    })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        TotalCompletedTasks = g.Count()
                    })
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.Month)
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Monthly Completion Retrieved Successfully",
                    Data = monthlyCompletion
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving monthly completion",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 14) Display Role Wise Active User Count.
        [HttpGet]
        public async Task<IActionResult> GetRoleWiseActiveUserCount()
        {
            try
            {
                var result = await _context.UserRole
                    .Where(x => x.User.IsActive)
                    .GroupBy(x => x.Role.RoleName)
                    .Select(g => new
                    {
                        RoleName = g.Key,
                        ActiveUsers = g.Count()
                    })
                    .OrderByDescending(x => x.ActiveUsers)
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Role Wise Active User Count Retrieved Successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving role wise active user count",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 15) Display each role with users assigned to it.
        [HttpGet]
        public async Task<IActionResult> GetUsersByRole()
        {
            try
            {
                var result = await _context.UserRole
                    .GroupBy(x => x.Role.RoleName)
                    .Select(g => new
                    {
                        RoleName = g.Key,
                        Users = g.Select(x => x.User.FullName).ToList()
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Users By Role Retrieved Successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving users by role",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 16) List Roles Having More Than 10 Users.
        [HttpGet]
        public async Task<IActionResult> GetRolesWithMoreThanTenUsers()
        {
            try
            {
                var result = await _context.UserRole
                    .GroupBy(x => x.Role.RoleName)
                    .Select(g => new
                    {
                        RoleName = g.Key,
                        TotalUsers = g.Count()
                    })
                    .Where(x => x.TotalUsers > 10)
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Roles With More Than 10 Users Retrieved Successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving roles with more than 10 users",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 17) Display role statistics.
        [HttpGet]
        public async Task<IActionResult> GetRoleStatistics()
        {
            try
            {
                var result = await _context.UserRole
                    .GroupBy(x => x.Role.RoleName)
                    .Select(g => new
                    {
                        RoleName = g.Key,
                        TotalUsers = g.Count(),
                        ActiveUsers = g.Count(x => x.User.IsActive),
                        InactiveUsers = g.Count(x => !x.User.IsActive)
                    })
                    .OrderByDescending(x => x.TotalUsers)
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Role Statistics Retrieved Successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving role statistics",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 18) Show tasks due within next 7 days.
        [HttpGet]
        public async Task<IActionResult> GetUpcomingDueTasks()
        {
            try
            {
                var result = await _context.Task
                    .Where(x =>
                        x.TaskDueDate >= DateTime.Today &&
                        x.TaskDueDate <= DateTime.Today.AddDays(7))
                    .Select(x => new
                    {
                        TaskID = x.TaskID,
                        TaskTitle = x.TaskTitle,
                        Project = x.ProjectAllocation.ProjectMaster.ProjectTitle,
                        Student = x.ProjectAllocation.Student.FullName,
                        TaskDueDate = x.TaskDueDate,
                        RemainingDays = EF.Functions.DateDiffDay(DateTime.Today, x.TaskDueDate)
                    })
                    .OrderBy(x => x.TaskDueDate)
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Upcoming Due Tasks Retrieved Successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving upcoming due tasks",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 19) Display each project with total tasks, completed tasks, pending tasks, and average task progress.
        [HttpGet]
        public async Task<IActionResult> GetProjectTaskSummary()
        {
            try
            {
                var result = await _context.Task
                    .GroupBy(x => x.ProjectAllocation.ProjectMaster.ProjectTitle)
                    .Select(g => new
                    {
                        Project = g.Key,
                        TotalTasks = g.Count(),
                        CompletedTasks = g.Count(x => x.TaskStatus.TaskStatusName == "Completed"),
                        PendingTasks = g.Count(x => x.TaskStatus.TaskStatusName == "Pending"),
                        AverageProgress = g.Average(x => x.ProgressPercentage)
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Project Task Summary Retrieved Successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving project task summary",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 20) Display project-wise total assigned score, earned score, and score percentage.
        [HttpGet]
        public async Task<IActionResult> GetProjectScoreSummary()
        {
            try
            {
                var result = await _context.Task
                    .GroupBy(x => x.ProjectAllocation.ProjectMaster.ProjectTitle)
                    .Select(g => new
                    {
                        Project = g.Key,
                        TotalAssignedScore = g.Sum(x => x.AssignedScore),
                        TotalEarnedScore = g.Sum(x => x.EarnedScore ?? 0),
                        ScorePercentage = g.Sum(x => x.AssignedScore) > 0
                            ? (g.Sum(x => x.EarnedScore ?? 0) / g.Sum(x => x.AssignedScore)) * 100
                            : 0
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Project Score Summary Retrieved Successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving project score summary",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 21) Display Top 10 projects based on average earned score.
        [HttpGet]
        public async Task<IActionResult> GetTopProjectsByScore()
        {
            try
            {
                var result = await _context.Task
                    .Where(x => x.EarnedScore != null)
                    .GroupBy(x => x.ProjectAllocation.ProjectMaster.ProjectTitle)
                    .Select(g => new
                    {
                        Project = g.Key,
                        AverageScore = g.Average(x => x.EarnedScore)
                    })
                    .OrderByDescending(x => x.AverageScore)
                    .Take(10)
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Top Projects By Score Retrieved Successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving top projects by score",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 22) Show project count, task count, and average progress for each faculty.
        [HttpGet]
        public async Task<IActionResult> GetFacultyStatistics()
        {
            try
            {
                var result = await _context.ProjectAllocation
                    .GroupBy(x => x.Faculty.FullName)
                    .Select(g => new
                    {
                        Faculty = g.Key,
                        TotalProjects = g.Count(),
                        TotalTasks = g.Sum(x => x.TotalTasksGiven),
                        AverageProgress = g.Average(x => x.ProgressPercentage)
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Faculty Statistics Retrieved Successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving faculty statistics",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 23) Display task completion statistics and average score for each student.
        [HttpGet]
        public async Task<IActionResult> GetStudentTaskStatistics()
        {
            try
            {
                var result = await _context.Task
                    .GroupBy(x => x.ProjectAllocation.Student.FullName)
                    .Select(g => new
                    {
                        Student = g.Key,
                        TotalTasks = g.Count(),
                        CompletedTasks = g.Count(x => x.TaskStatus.TaskStatusName == "Completed"),
                        PendingTasks = g.Count(x => x.TaskStatus.TaskStatusName == "Pending"),
                        AverageScore = g.Average(x => x.EarnedScore)
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Student Task Statistics Retrieved Successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving student task statistics",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 24) Display projects whose expected completion date has passed but are still incomplete.
        [HttpGet]
        public async Task<IActionResult> GetOverdueProjects()
        {
            try
            {
                var result = await _context.ProjectAllocation
                    .Where(x =>
                        x.ProjectEndDate < DateTime.Now &&
                        x.ProgressPercentage < 100)
                    .Select(x => new
                    {
                        ProjectTitle = x.ProjectMaster.ProjectTitle,
                        Student = x.Student.FullName,
                        Faculty = x.Faculty.FullName,
                        ProjectEndDate = x.ProjectEndDate,
                        ProgressPercentage = x.ProgressPercentage
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Overdue Projects Retrieved Successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving overdue projects",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 25) Show month-wise completed task count (alternative projection).
        [HttpGet]
        public async Task<IActionResult> GetMonthlyCompletedTaskCount()
        {
            try
            {
                var result = await _context.Task
                    .Where(x => x.TaskCompletedTime != null)
                    .GroupBy(x => new
                    {
                        Year = x.TaskCompletedTime.Value.Year,
                        Month = x.TaskCompletedTime.Value.Month
                    })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        CompletedTasks = g.Count()
                    })
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.Month)
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Monthly Completed Task Count Retrieved Successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving monthly completed task count",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 26) Rank faculties based on average project progress.
        [HttpGet]
        public async Task<IActionResult> GetFacultyRankingsByProgress()
        {
            try
            {
                var result = await _context.ProjectAllocation
                    .GroupBy(x => x.Faculty.FullName)
                    .Select(g => new
                    {
                        Faculty = g.Key,
                        AverageProgress = g.Average(x => x.ProgressPercentage)
                    })
                    .OrderByDescending(x => x.AverageProgress)
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Faculty Rankings By Progress Retrieved Successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving faculty rankings by progress",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // 27) Display task statistics for every project.
        [HttpGet]
        public async Task<IActionResult> GetProjectTaskStatistics()
        {
            try
            {
                var result = await _context.Task
                    .GroupBy(x => x.ProjectAllocation.ProjectMaster.ProjectTitle)
                    .Select(g => new
                    {
                        Project = g.Key,
                        TotalTasks = g.Count(),
                        CompletedTasks = g.Count(x => x.TaskStatus.TaskStatusName == "Completed"),
                        PendingTasks = g.Count(x => x.TaskStatus.TaskStatusName == "Pending"),
                        OverdueTasks = g.Count(x =>
                            x.TaskDueDate < DateTime.Now &&
                            x.TaskStatus.TaskStatusName != "Completed")
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Project Task Statistics Retrieved Successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error occurred while retrieving project task statistics",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
