using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPMS_API.Models
{
    public class TaskStatus
    {
        [Key]
        public int TaskStatusID { get; set; }
        [Required, MaxLength(20)]
        public string TaskStatusName { get; set; } = string.Empty;

        [Required, MaxLength(250)]
        public string TaskStatusCssClass { get; set; } = string.Empty;
    }
}
