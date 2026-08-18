namespace SPMS_API.DTOs
{
    public class ReadTask
    {
        public int TaskID { get; set; }
        public int ProjectAllocationID { get; set; }
        public string? ProjectTitle { get; set; }
        public string? StudentName { get; set; }
        public string TaskTitle { get; set; }
        public string? TaskDescription { get; set; }
        public int TaskStatusID { get; set; }
        public string? TaskStatusName { get; set; }
        public int TaskPriorityID { get; set; }
        public string? TaskPriorityName { get; set; }
        public decimal AssignedScore { get; set; }
        public decimal? EarnedScore { get; set; }
        public decimal ProgressPercentage { get; set; }
        public DateTime TaskAssignedDate { get; set; }
        public DateTime? TaskStartDate { get; set; }

        public DateTime? TaskEndDate { get; set; }
        public DateTime? TaskDueDate { get; set; }
        public DateTime? TaskCompletedDate { get; set; }
        public DateTime? NextFollowUpDate { get; set; }
        public string? FacultyRemarks { get; set; }
        public string? StudentRemarks { get; set; }
    }


    public class CreateTask
    {
        public int ProjectAllocationID { get; set; }

        public string TaskTitle { get; set; } = string.Empty;

        public string? TaskDescription { get; set; }

        public int TaskStatusID { get; set; }

        public int TaskPriorityID { get; set; }

        public decimal AssignedScore { get; set; }

        public decimal? EarnedScore { get; set; }

        public decimal ProgressPercentage { get; set; }

        public DateTime TaskAssignnedDate { get; set; } = DateTime.Now;

        public DateTime? TaskStartDate { get; set; }

        public DateTime? TaskDueDate { get; set; }

        public DateTime? TaskEndDate { get; set; }

        public DateTime? TaskCompletedTime { get; set; }

        public DateTime? NextFollowUpDate { get; set; }

        public string? FacultyRemarks { get; set; }

        public string? StudentRemarks { get; set; }
    }


    public class UpdateTask
    {
        public int ProjectAllocationID { get; set; }

        public string TaskTitle { get; set; } = string.Empty;

        public string? TaskDescription { get; set; }

        public int TaskStatusID { get; set; }

        public int TaskPriorityID { get; set; }

        public decimal AssignedScore { get; set; }

        public decimal? EarnedScore { get; set; }

        public decimal ProgressPercentage { get; set; }

        public DateTime TaskAssignnedDate { get; set; }

        public DateTime? TaskStartDate { get; set; }

        public DateTime? TaskDueDate { get; set; }

        public DateTime? TaskEndDate { get; set; }

        public DateTime? TaskCompletedTime { get; set; }

        public DateTime? NextFollowUpDate { get; set; }

        public string? FacultyRemarks { get; set; }

        public string? StudentRemarks { get; set; }
    }
}