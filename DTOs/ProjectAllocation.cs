namespace SPMS_API.DTOs
{
    public class ReadProjectAllocation
    {
        public int ProjectAllocationID { get; set; }
        public int ProjectID { get; set; }
        public string? ProjectTitle { get; set; }
        public int StudentID { get; set; }
        public string? StudentName { get; set; }
        public int FacultyID { get; set; }
        public string? FacultyName { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime ProjectStartDate { get; set; }
        public DateTime ProjectEndDate { get; set; }
        public int TotalTasksGiven { get; set; }
        public int TotalCompletedTasks { get; set; }
        public decimal ProgressPercentage { get; set; }
        public string? OverAllGrade { get; set; }
    }


    public class CreateProjectAllocation
    {
        public int ProjectID { get; set; }

        public int StudentID { get; set; }

        public int FacultyID { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.Now;

        public DateTime ProjectStartDate { get; set; }

        public DateTime ProjectEndDate { get; set; }

        public int TotalTasksGiven { get; set; }

        public int TotalCompletedTasks { get; set; }

        public decimal ProgressPercentage { get; set; }

        public string? OverAllGrade { get; set; }
    }


    public class UpdateProjectAllocation
    {
        public int ProjectID { get; set; }

        public int StudentID { get; set; }

        public int FacultyID { get; set; }

        public DateTime AssignedDate { get; set; }

        public DateTime ProjectStartDate { get; set; }

        public DateTime ProjectEndDate { get; set; }

        public int TotalTasksGiven { get; set; }

        public int TotalCompletedTasks { get; set; }

        public decimal ProgressPercentage { get; set; }

        public string? OverAllGrade { get; set; }
    }
}