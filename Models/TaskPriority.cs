using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPMS_API.Models
{
    public class TaskPriority
    {
        [Key]
        public int TaskPriorityId { get; set; }

        [Required, MaxLength(20)]
        public string TaskPriorityName { get; set; } = string.Empty;


        [Required, MaxLength(20)]
        public string TaskPriorityCssClass { get; set; } = string.Empty;
    }
}
