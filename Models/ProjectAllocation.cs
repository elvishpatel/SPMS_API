using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPMS_API.Models
{
    public class ProjectAllocation
    {
        [Key]
        public int ProjectAllocationID { get; set; }

        public ProjectMaster? ProjectMaster { get; set; }

        [Required]
        [ForeignKey("ProjectMaster")]
        public int ProjectID { get; set; }

        public User? Student { get; set; }

        [Required]
        [ForeignKey("Student")]
        public int StudentID { get; set; }

        public User? Faculty { get; set; }

        [Required]
        [ForeignKey("Faculty")]
        public int FacultyID { get; set; }

        [Required]
        public DateTime AssignedDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime ProjectStartDate { get; set; }

        [Required]
        public DateTime ProjectEndDate { get; set; }

        [Required]
        public int TotalTasksGiven { get; set; }

        [Required]
        public int TotalCompletedTasks { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal ProgressPercentage { get; set; }

        [StringLength(1)]
        [RegularExpression("^[ABC]?$", ErrorMessage = "Grade must be A, B, or C.")]
        public string? OverAllGrade { get; set; }
    }
}

