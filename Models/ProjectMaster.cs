using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPMS_API.Models
{
    public class ProjectMaster
    {
        [Key]
        public int ProjectId { get; set; }

        [Required, MaxLength(200)]
        public string ProjectTitle { get; set; }

        [MaxLength(250)]
        public string? Description { get; set; }
    }
}
