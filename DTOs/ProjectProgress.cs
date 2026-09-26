namespace SPMS_API.DTOs
{
    /// <summary>
    /// Fields a faculty member may update for a project allocated to them.
    /// Allocation ownership and project/student links remain admin-only.
    /// </summary>
    public class UpdateProjectProgress
    {
        public int TotalTasksGiven { get; set; }
        public int TotalCompletedTasks { get; set; }
        public decimal ProgressPercentage { get; set; }
        public string? OverAllGrade { get; set; }
    }
}
